using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;


public class BootException : Exception
{
    public string stepName { get; }

    public BootException(string stepName, Exception inner) : base($"Boot failed at step '{stepName}'", inner)
    {
        this.stepName = stepName;
    }
}

public class Launch : MonoBehaviour
{
    private sealed class SimpleProgress : IProgress<float>
    {
        private readonly Action<float> _handler;
        public SimpleProgress(Action<float> handler) => _handler = handler;
        public void Report(float value) => _handler(value);
    }

    private readonly struct BootStep
    {
        public readonly string Name;
        public readonly float Weight;
        public readonly Func<IProgress<float>, CancellationToken, UniTask> Run;

        public BootStep(string name, float weight, Func<IProgress<float>, CancellationToken, UniTask> run)
        {
            Name = name;
            Weight = weight;
            Run = run;
        }
    }

    private AppContext _app;
    private GameContext _context;
    public ISceneService sceneService => _context.SceneService;

    private BootStep[] _steps;
    private bool _isRunning;

    public void Begin(AppContext app)
    {
        Debug.Log("Launch Begin");

        _app = app;
        _context = Session.Begin(app);

        _steps = new[]
        {
            new BootStep("Addressables Init", 0.25f, InitializeAddressablesAsync),
            new BootStep("Localization Init", 0.10f, InitializeLocalizationAsync),
            new BootStep("Preload Assets",    0.25f, LoadPreloadAssetsAsync),
            new BootStep("Load Data",         0.20f, InitializeLoadAsync),
            new BootStep("Server Init",       0.20f, InitializeServerAsync),
        };
    }

    public async UniTask RunAsync(BootController bootController, CancellationToken ct)
    {
        if (_isRunning == true)
        {
            throw new InvalidOperationException("Boot already running");
        }
        _isRunning = true;

        try
        {
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyCancellationToken);
            var token = linked.Token;

            float totalWeight = _steps.Sum(s => s.Weight);
            float start = 0f;

            foreach (var step in _steps)
            {
                token.ThrowIfCancellationRequested();

                float range = step.Weight / totalWeight;
                float capturedStart = start;

                var progress = new SimpleProgress(p => bootController.SetProgress(capturedStart + Mathf.Clamp01(p) * range));

                try
                {
                    await step.Run(progress, token);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    throw new BootException(step.Name, e);
                }

                start += range;
                bootController.SetProgress(start);
            }

            token.ThrowIfCancellationRequested();
            Complete();
        }
        finally
        {
            _isRunning = false;
        }
    }

    private async UniTask InitializeAddressablesAsync(IProgress<float> progress, CancellationToken ct)
    {
        var handle = Addressables.InitializeAsync(false);
        try
        {
            await TrackProgressAsync(handle, progress, ct);
            ThrowIfFailed(handle, "Addressables initialization");
        }
        finally
        {
            if (handle.IsValid() == true)
            {
                Addressables.Release(handle);
            }
        }
    }

    private async UniTask InitializeLocalizationAsync(IProgress<float> progress, CancellationToken ct)
    {
        var handle = LocalizationSettings.InitializationOperation;
        await TrackProgressAsync(handle, progress, ct);
        ThrowIfFailed(handle, "Localization initialization");
    }

    private async UniTask LoadPreloadAssetsAsync(IProgress<float> progress, CancellationToken ct)
    {
        var preload = _context.Preload;
        preload.Release();

        string[] labels = { "Preload" };

        var locations = new List<IResourceLocation>();

        foreach (var label in labels)
        {
            var handle = Addressables.LoadResourceLocationsAsync(label);

            try
            {
                await UniTask.WaitUntil(() => handle.IsDone, cancellationToken: ct);
                ThrowIfFailed(handle, $"Resolve preload label '{label}'");

                locations.AddRange(handle.Result);
            }
            finally
            {
                if (handle.IsValid() == true)
                {
                    Addressables.Release(handle);
                }
            }
        }

        var distinct = locations.Distinct().ToList();
        if (distinct.Count == 0)
        {
            return;
        }

        foreach (var loc in distinct)
        {
            preload.Add(Addressables.LoadAssetAsync<UnityEngine.Object>(loc));
        }

        try
        {
            await TrackProgressAsync(preload.Handles, progress, ct);

            var failed = preload.Handles.Where(h => h.Status != AsyncOperationStatus.Succeeded).ToList();
            if (failed.Count > 0)
            {
                throw new InvalidOperationException($"Preload failed: {failed.Count}/{preload.Handles.Count} assets", failed[0].OperationException);
            }
        }
        catch
        {
            preload.Release();
            throw;
        }
    }

    private async UniTask InitializeLoadAsync(IProgress<float> progress, CancellationToken ct)
    {
        const int stepCount = 2;
        int done = 0;

        await _context.GameDataLoader.LoadAsync();
        ct.ThrowIfCancellationRequested();
        progress.Report(++done / (float)stepCount);

        await _context.Storage.LoadAsync();
        ct.ThrowIfCancellationRequested();
        _app.ApplyDeviceSettings();
        progress.Report(++done / (float)stepCount);
    }

    private async UniTask InitializeServerAsync(IProgress<float> progress, CancellationToken ct)
    {
        var request = new InitializeRequest();

        var response = await _context.GameServer.InitializeAsync(request);
        ct.ThrowIfCancellationRequested();

        _context.User.InitializeServer(response.user);
        _context.MercenaryService.InitializeServer(response.mercenaries);
        _context.PurchaseService.InitializeServer(response.products);
        _context.GachaService.InitializeServer(response.gachas);

        progress.Report(1f);
    }

    private async UniTask TrackProgressAsync(AsyncOperationHandle handle, IProgress<float> progress, CancellationToken ct)
    {
        while (handle.IsDone == false)
        {
            progress.Report(handle.PercentComplete);
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }

        progress.Report(1f);
    }

    private async UniTask TrackProgressAsync<T>(IReadOnlyList<AsyncOperationHandle<T>> handles, IProgress<float> progress, CancellationToken ct)
    {
        while (true)
        {
            float sum = 0f;
            bool done = true;

            foreach (var handle in handles)
            {
                sum += handle.PercentComplete;
                if (handle.IsDone == false)
                {
                    done = false;
                }
            }

            progress.Report(sum / handles.Count);

            if (done)
            {
                break;
            }

            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }

        progress.Report(1f);
    }

    private static void ThrowIfFailed(AsyncOperationHandle handle, string what)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            return;
        }

        throw new InvalidOperationException($"{what} failed", handle.OperationException);
    }

    private void Complete()
    {
        _context.User.RunGame();
        _context.SoundService.PlayBGM(SoundId.BGM_BOOT);
    }
}