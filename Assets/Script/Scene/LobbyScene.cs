using System.Reflection;
using UnityEngine;


public class LobbySceneContext
{
    public IAssetLoader assetLoader;
    public IPopupService popupService;
    public ICurrencyService currencyService;
    public ISceneLoader sceneLoader;
    public StageService stageService;
    public User user;
    public ProductStorage productStorage;
    public PurchaseService purchaseService;
}

public class LobbyScene : MonoBehaviour
{
    private LobbySceneContext _context;

    [SerializeField]
    private UILobby _uiLobby;

    private UILobbyPresenter _uiLobbyPresenter;

    private void Awake()
    {
        Debug.Log(this.GetType().Name + " " + MethodBase.GetCurrentMethod().Name);
    }

    private void OnDestroy()
    {
        _uiLobbyPresenter?.Dispose();
    }

    public void InitDependencies(LobbySceneContext context)
    {
        _context = context;
    }

    public void Initialize()
    {
        _uiLobbyPresenter = new UILobbyPresenter(_uiLobby, new UILobbyContext
        {
            assetLoader = _context.assetLoader,
            popupService = _context.popupService,
            currencyService = _context.currencyService,
            sceneLoader = _context.sceneLoader,
            stageService = _context.stageService,
            user = _context.user,
            productStorage = _context.productStorage,
            purchaseService = _context.purchaseService,
        });
        _uiLobbyPresenter.Initialize();
    }
}