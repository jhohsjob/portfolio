using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;


public interface ISceneService
{
    void LoadBootScene();
    void LoadLobbyScene();
    void LoadBattleScene();
    void Dispose();
}

public static class SceneNames
{
    public const string Boot = "01.BootScene";   // 실제 빌드 세팅의 씬 이름으로 맞추세요
    public const string Lobby = "02.LobbyScene";
    public const string Battle = "03.BattleScene";
}

public class SceneServiceContext
{
    public IAssetService assetService;
    public ISoundService soundService;
    public IDeviceSettings deviceSettings;
    public User user;
    public ICurrencyService currencyService;
    public MercenaryService mercenaryService;
    public PurchaseService purchaseService;
    public GachaService gachaService;
    public StageService stageService;
    public IPopupService popupService;
}

public class SceneService : ISceneService, IDisposable
{
    private SceneServiceContext _context;

    private readonly CancellationTokenSource _cts = new();

    private bool _isLoading;
    private bool _disposed;

    public SceneService(SceneServiceContext context)
    {
        _context = context;
    }

    public void LoadBootScene() => TransitionAsync(SceneNames.Boot, inject: null, bgm: null).Forget();

    public void LoadLobbyScene() => TransitionAsync(SceneNames.Lobby, InjectLobbyScene, SoundId.BGM_LOBBY).Forget();

    public void LoadBattleScene() => TransitionAsync(SceneNames.Battle, InjectBattleScene, SoundId.BGM_BATTLE).Forget();

    public void Dispose()
    {
        if (_disposed == true)
        {
            return;
        }
        _disposed = true;

        _cts.Cancel();
        _cts.Dispose();
    }

    private async UniTask TransitionAsync(string sceneName, Action inject, int? bgm)
    {
        if (_disposed == true)
        {
            return;
        }

        if (_isLoading == true)
        {
            Debug.LogWarning($"Scene transition already in progress. Ignored: {sceneName}");
            return;
        }

        _isLoading = true;
        var ct = _cts.Token;

        try
        {
            _context.soundService.StopBGM();
            await UniTask.Yield(ct);

            _context.popupService.CloseAllPopup();
            await UniTask.Yield(ct);

            var op = SceneManager.LoadSceneAsync(sceneName);
            if (op == null)
            {
                throw new InvalidOperationException($"Failed to start loading scene '{sceneName}'");
            }

            await op.ToUniTask(cancellationToken: ct);

            ct.ThrowIfCancellationRequested();

            inject?.Invoke();

            if (bgm.HasValue)
            {
                _context.soundService.PlayBGM(bgm.Value);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void InjectLobbyScene()
    {
        var lobbyScene = UnityEngine.Object.FindAnyObjectByType<LobbyScene>();
        if (lobbyScene == null)
        {
            Debug.LogError("LobbyScene not found");
            return;
        }

        lobbyScene.InitDependencies(new LobbySceneContext
        {
            assetService = _context.assetService,
            soundService = _context.soundService,
            deviceSettings = _context.deviceSettings,
            user = _context.user,
            currencyService = _context.currencyService,
            mercenaryService = _context.mercenaryService,
            purchaseService = _context.purchaseService,
            gachaService = _context.gachaService,
            stageService = _context.stageService,
            sceneService = this,
            popupService = _context.popupService,
        });
        lobbyScene.Initialize();
    }

    private void InjectBattleScene()
    {
        var battleScene = UnityEngine.Object.FindAnyObjectByType<BattleScene>();
        if (battleScene == null)
        {
            Debug.LogError("BattleScene not found");
            return;
        }

        battleScene.InitDependencies(new BattleSceneContext
        {
            assetService = _context.assetService,
            user = _context.user,
            sceneLoader = this,
            currencyService = _context.currencyService,
            stageService = _context.stageService,
            popupService = _context.popupService,
        });
        battleScene.Initialize();
    }
}