using System.Reflection;
using UnityEngine;


public class LobbyScene : MonoBehaviour
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
        public ISceneService sceneService;
        public IPopupService popupService;
    }


    private Deps _deps;

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

    public void InitDependencies(Deps context)
    {
        _deps = context;
    }

    public void Initialize()
    {
        _uiLobbyPresenter = new UILobbyPresenter(_uiLobby, new UILobbyPresenter.Deps
        {
            assetService = _deps.assetService,
            deviceSettings = _deps.deviceSettings,
            user = _deps.user,
            currencyService = _deps.currencyService,
            mercenaryService = _deps.mercenaryService,
            purchaseService = _deps.purchaseService,
            gachaService = _deps.gachaService,
            stageService = _deps.stageService,
            sceneService = _deps.sceneService,
            popupService = _deps.popupService,
        });
        _uiLobbyPresenter.Initialize();

#if DEBUG_MODE
        _deps.assetService.LoadPrefab("UIDebug", prefab =>
        {
            _uiDebug = Instantiate(prefab).GetComponent<UIDebug>();
            _uiDebug.Initialize(new UIDebugPresenter.Deps
            {
                user = _deps.user
            });
        });
#endif
    }
}