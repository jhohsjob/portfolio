using UnityEngine;


public class BattleSceneContext
{
    public IAssetLoader assetLoader;
    public IPopupService popupService;
    public ISceneLoader sceneLoader;
    public ICurrencyService currencyService;
    public StageService stageService;
    public User user;
}

public class BattleScene : MonoBehaviour
{
    private BattleSceneContext _context;
    private BattleManager _battleManager;

    [SerializeField]
    private UIBattle _uiBattle;
    [SerializeField]
    private Map _map;
    [SerializeField]
    private Player _player;
    [SerializeField]
    private PlayerCamera _playerCamera;

    [SerializeField]
    private Transform _actorContainer;

    private UIBattlePresenter _uiBattlePresenter;

    private void Awake()
    {
    }

    private void OnDestroy()
    {
        _uiBattlePresenter?.Dispose();
    }

    public void InitDependencies(BattleSceneContext context)
    {
        _context = context;
    }

    public void Initialize()
    {
        InitializeBattleManager();

        InitializeUIBattle();

        InitializePlayer();

        InitializeMap();
        
        _battleManager.StartBattle();
    }

    private void InitializeBattleManager()
    {
        var go = new GameObject("BattleManager");
        _battleManager = go.AddComponent<BattleManager>();
        _battleManager.Initialize(new BattleManagerContext
        {
            user = _context.user,
            stageService = _context.stageService,
            currencyService = _context.currencyService,
            actorContainer = _actorContainer,
            playerTransform = _player.transform,
            getSpawnPosition = GetEnemySpawnPosition
        });
    }

    private void InitializeUIBattle()
    {
        _uiBattlePresenter = new UIBattlePresenter(_uiBattle, new UIBattleContext
        {
            assetLoader = _context.assetLoader,
            popupService = _context.popupService,
            sceneLoader = _context.sceneLoader,
            battleController = _battleManager.battleController,
            battleReward = _battleManager.battleReward,
            dashController = _player.dash,
            onDashAction = _player.HandleDashAction,
            onJoystickAction = _player.HandleJoystickAction
        });
        _uiBattlePresenter.Initialize();
    }

    private void InitializePlayer()
    {
        _player.InitDependencies(new PlayerContext()
        {
            battleState = _battleManager.battleState,
            assetLoader = _context.assetLoader,
            actorSpawner = _battleManager.actorSpawner,
            GetNearestEnemy = _battleManager.GetNearestEnemy
        });

        var mercenary = GameSession.instance.currentMercenary;
        _player.Init(mercenary);
        if (_player.moveBehaviour is DirectionMove direction)
        {
            direction.Setup(_player.inputSource, _map.GetBounds);
        }
        int nextId = _battleManager.GetNextActorID(mercenary.id);
        int sortingOrder = _battleManager.GetNextOrderInLayer(mercenary.roleType);
        _player.Enter(nextId, sortingOrder, HandlePlayerDied);

        _playerCamera.Init(_uiBattle.GetTopUIHeight(), GetMapBounds);

        _player.onGoldCollected += _battleManager.HandleGoldCollected;
    }

    private void InitializeMap()
    {
        _map.Init(_battleManager.GetMapOriginal());
    }

    public Bounds GetMapBounds()
    {
        return _map.GetBounds();
    }

    public Vector3 GetEnemySpawnPosition()
    {
        return _map.GetRandomPositionAwayFromTarget(_player.transform.position);
    }

    private void HandlePlayerDied(ActorBase actor)
    {
        if (actor is not Player)
        {
            return;
        }

        _battleManager.LoseEndBattle();
    }
}