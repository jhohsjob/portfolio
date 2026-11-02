using System.Reflection;
using UnityEngine;


public class LobbySceneContext
{
    public IAssetService assetService;
    public IPopupService popupService;
    public ICurrencyService currencyService;
    public ISceneLoader sceneLoader;
    public StageService stageService;
    public User user;
    public MercenaryService mercenaryService;
    public PurchaseService purchaseService;
    public GachaService gachaService;
}

public class LobbyScene : MonoBehaviour
{
    private LobbySceneContext _context;

    [SerializeField]
    private UILobby _uiLobby;

    private UILobbyPresenter _uiLobbyPresenter;

#if DEBUG_MODE
    private UIDebug _uiDebug;
#endif

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
            assetService = _context.assetService,
            popupService = _context.popupService,
            currencyService = _context.currencyService,
            sceneLoader = _context.sceneLoader,
            stageService = _context.stageService,
            user = _context.user,
            mercenaryService = _context.mercenaryService,
            purchaseService = _context.purchaseService,
            gachaService = _context.gachaService
        });
        _uiLobbyPresenter.Initialize();

#if DEBUG_MODE
        _context.assetService.LoadPrefab("UIDebug", prefab =>
        {
            _uiDebug = Instantiate(prefab).GetComponent<UIDebug>();
            _uiDebug.Initialize(new UIDebugContext
            {
                user = _context.user
            });
        });
#endif
    }
}