using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;


public interface ISceneService
{
    void LoadBootScene();
    void LoadLobbyScene();
    void LoadBattleScene(BattleRequest request);
    void Dispose();
}

public static class SceneNames
{
    public const string Boot = "01.BootScene";
    public const string Lobby = "02.LobbyScene";
    public const string Battle = "03.BattleScene";
}

public class SceneService : ISceneService, IDisposable
{
    public sealed class Deps
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

        public MonsterManager monsterManager;
        public ProjectileManager projectileManager;
        public DropItemManager dropItemManager;
    }


    private BattleRequest _battleRequest;

    private Deps _deps;

    private readonly CancellationTokenSource _cts = new();

    private bool _isLoading;
    private bool _disposed;

    public SceneService(Deps context)
    {
        _deps = context;
    }

    public void LoadBootScene() => TransitionAsync(SceneNames.Boot, inject: null, bgm: null).Forget();

    public void LoadLobbyScene() => TransitionAsync(SceneNames.Lobby, InjectLobbyScene, SoundId.BGM_LOBBY).Forget();

    public void LoadBattleScene(BattleRequest request)
    {
        _battleRequest = request;
        TransitionAsync(SceneNames.Battle, InjectBattleScene, SoundId.BGM_BATTLE).Forget();
    }

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
            _deps.soundService.StopBGM();
            await UniTask.Yield(ct);

            _deps.popupService.CloseAllPopup();
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
                _deps.soundService.PlayBGM(bgm.Value);
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

        lobbyScene.InitDependencies(new LobbyScene.Deps
        {
            assetService = _deps.assetService,
            soundService = _deps.soundService,
            deviceSettings = _deps.deviceSettings,
            user = _deps.user,
            currencyService = _deps.currencyService,
            mercenaryService = _deps.mercenaryService,
            purchaseService = _deps.purchaseService,
            gachaService = _deps.gachaService,
            stageService = _deps.stageService,
            sceneService = this,
            popupService = _deps.popupService,

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

        battleScene.InitDependencies(new BattleScene.Deps
        {
            assetService = _deps.assetService,
            user = _deps.user,
            sceneLoader = this,
            stageService = _deps.stageService,
            popupService = _deps.popupService,

            monsterManager = _deps.monsterManager,
            projectileManager = _deps.projectileManager,
            dropItemManager = _deps.dropItemManager,
            request = _battleRequest,
        });
        battleScene.Initialize();
    }
}