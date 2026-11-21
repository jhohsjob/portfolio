using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;


public class BootScene : MonoBehaviour
{
    [SerializeField]
    private BootLauncher _launcher;
    [SerializeField]
    private GlobalRoot _global;
    [SerializeField]
    private UIBoot _uiBoot;

    private BootProgress _bootProgress = new();
    private BootPresenter _bootPresenter;

    private CancellationTokenSource _cts;

    private void Awake()
    {
        Debug.Log("BootScene Awake");

        var app = AppRoot.EnsureCreated(_global);
        _launcher.Begin(app);
    }

    private async void Start()
    {
        Debug.Log("BootScene Start");

        _bootPresenter = new BootPresenter(_uiBoot, _bootProgress, _launcher.sceneService);
        
        await BootAsync();
    }

    private void Update()
    {
        _bootProgress?.Update(Time.deltaTime);
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _bootPresenter?.Dispose();
    }

    private async UniTask BootAsync()
    {
        _cts = new CancellationTokenSource();

        try
        {
            await _launcher.RunAsync(_bootProgress, _cts.Token);
            _bootProgress.Complete();
        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            Debug.LogException(e);
            _bootProgress.Fail(e);
        }
    }
}