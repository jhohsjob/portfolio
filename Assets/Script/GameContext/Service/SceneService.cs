using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public interface ISceneLoader
{
    void LoadLobbyScene();
    void LoadBattleScene();
}

public class SceneServiceContext
{
    public IAssetLoader assetLoader;
    public IPopupService popupService;
    public ICurrencyService currencyService;
    public StageService stageService;
    public User user;
    public ProductStorage productStorage;
    public PurchaseService purchaseService;
}

public class SceneService : ISceneLoader
{
    private SceneServiceContext _context;

    public SceneService(SceneServiceContext context)
    {
        _context = context;
    }

    public void LoadBattleScene()
    {
        IEnumeratorTool.instance.StartCoroutine(coLoadBattleScene());
    }

    private IEnumerator coLoadBattleScene()
    {
        yield return null;
        _context.popupService.CloseAllPopup();

        yield return null;
        var op = SceneManager.LoadSceneAsync("03.BattleScene");
        //op.allowSceneActivation = false;

        while (op.isDone == false)
        {
            Debug.Log(op.progress);

            yield return null;
        }

        InjectBattleScene();
    }

    private void InjectBattleScene()
    {
        var battleScene = Object.FindAnyObjectByType<BattleScene>();
        if (battleScene == null)
        {
            Debug.LogError("BattleScene not found");
            return;
        }

        battleScene.InitDependencies(new BattleSceneContext
        {
            assetLoader = _context.assetLoader,
            popupService = _context.popupService,
            sceneLoader = this,
            currencyService = _context.currencyService,
            stageService = _context.stageService,
            user = _context.user
        });
        battleScene.Initialize();
    }

    public void LoadLobbyScene()
    {
        IEnumeratorTool.instance.StartCoroutine(coLoadLobbyScene());
    }

    private IEnumerator coLoadLobbyScene()
    {
        yield return null;
        _context.popupService.CloseAllPopup();

        yield return null;
        var op = SceneManager.LoadSceneAsync("02.LobbyScene");
        //op.allowSceneActivation = false;

        while (op.isDone == false)
        {
            Debug.Log(op.progress);

            yield return null;
        }

        InjectLobbyScene();

        yield return null;
    }

    private void InjectLobbyScene()
    {
        var lobbyScene = Object.FindAnyObjectByType<LobbyScene>();
        if (lobbyScene == null)
        {
            Debug.LogError("LobbyScene not found");
            return;
        }

        lobbyScene.InitDependencies(new LobbySceneContext
        {
            assetLoader = _context.assetLoader,
            popupService = _context.popupService,
            currencyService = _context.currencyService,
            sceneLoader = this,
            stageService = _context.stageService,
            user = _context.user,
            productStorage = _context.productStorage,
            purchaseService = _context.purchaseService,
        });
        lobbyScene.Initialize();
    }
}