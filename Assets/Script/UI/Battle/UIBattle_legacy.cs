//using System.Collections;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;


//public class UIBattleContext
//{
//    public IAssetLoader assetLoader;
//    public IBattleState battleState;
//    public IBattleController battleController;
//    public IPopupService popupService;
//    public ISceneLoader sceneLoader;
//    public DashController dashController;
//    public BattleRewardRuntime battleReward;
//}

//public class UIBattle : MonoBehaviour
//{
//    private enum PanelMode { Start, Main, End }

//    private ISceneLoader _sceneLoader;

//    [SerializeField]
//    private UIBattleTop _top;
//    [SerializeField]
//    private RectTransform _topRect;
//    [SerializeField]
//    private GameObject _panelStartDirection;
//    [SerializeField]
//    private UIPanelBattleMain _panelBattleMain;
//    [SerializeField]
//    private GameObject _panelBattleEnd;

//    [SerializeField]
//    private TextMeshProUGUI _txtDirection;

//    [SerializeField]
//    private HPBarController _hpBarContainer;

//    [SerializeField]
//    private UIJoystick _joystick;

//    [SerializeField]
//    private Button _btnLobby;

//    private UIBattleTopPresenter _topPresenter;

//    private void Awake()
//    {
//        PanelChange(PanelMode.Start);

//        _btnLobby.onClick.AddListener(OnClickLobby);

//        EventHelper.AddEventListener(EventName.BattleStatus, OnBattleStatus);
//    }

//    private void OnDestroy()
//    {
//        _topPresenter.Dispose();

//        EventHelper.RemoveEventListener(EventName.BattleStatus, OnBattleStatus);
//    }

//    public void InitDependencies(UIBattleContext context, Player player)
//    {
//        _sceneLoader = context.sceneLoader;

//        _topPresenter = new UIBattleTopPresenter(_top, new UIBattleTopContext
//        {
//            battleController = context.battleController,
//            popupService = context.popupService,
//            sceneLoader = context.sceneLoader,
//            battleReward = context.battleReward
//        });
        
//        _panelBattleMain.InitDependencies(new UIPanelBattleMainContext
//        {
//            dashController = context.dashController,
//            battleState = context.battleState
//        });
        
//        _hpBarContainer.InitDependencies(context.assetLoader);

//        _joystick.onJoystickMove += player.SetJoystick;
//    }

//    public void Initialize()
//    {
//        _hpBarContainer.Initialize();
//    }

//    private IEnumerator StartDirection()
//    {
//        _txtDirection.text = "START";

//        yield return new WaitForSeconds(1f);

//        PanelChange(PanelMode.Main);
//    }

//    private void BattleWinDirection()
//    {
//        _txtDirection.text = "YOU Win";

//        PanelChange(PanelMode.End);
//    }

//    private void BattleLoseDirection()
//    {
//        _txtDirection.text = "YOU DIE";

//        PanelChange(PanelMode.End);
//    }

//    private void PanelChange(PanelMode mode)
//    {
//        switch (mode)
//        {
//            case PanelMode.Start:
//                _panelStartDirection.SetActive(true);
//                _panelBattleMain.Hide();
//                _panelBattleEnd.SetActive(false);
//                break;

//            case PanelMode.Main:
//                _panelStartDirection.SetActive(false);
//                _panelBattleMain.Show();
//                _panelBattleEnd.SetActive(false);
//                break;

//            case PanelMode.End:
//                _panelStartDirection.SetActive(true);
//                _panelBattleMain.Hide();
//                _panelBattleEnd.SetActive(true);
//                break;
//        }
//    }

//    public float GetTopUIHeight()
//    {
//        float screenHeight = Screen.height;
//        float camHeight = Camera.main.orthographicSize * 2f;

//        float uiRatio = _topRect.rect.height / screenHeight;

//        return camHeight * uiRatio;
//    }
    
//    private void OnClickLobby()
//    {
//        _sceneLoader.LoadLobbyScene();
//    }

//    private void OnBattleStatus(object sender, object data)
//    {
//        if (data == null || (data is BattleStatus) == false)
//        {
//            return;
//        }

//        switch ((BattleStatus)data)
//        {
//            case BattleStatus.Running:
//                StartCoroutine(StartDirection());
//                break;

//            case BattleStatus.Win:
//                BattleWinDirection();
//                break;

//            case BattleStatus.Lose:
//                BattleLoseDirection();
//                break;
//        }
//    }
//}
