using UnityEngine;


public class BattleScene : MonoBehaviour
{
    public sealed class Deps
    {
        public IAssetService assetService;
        public User user;
        public StageService stageService;
        public ISceneService sceneLoader;
        public IPopupService popupService;

        public MonsterCatalog monsterCatalog;
        public ProjectileCatalog projectileCatalog;
        public DropItemCatalog dropItemCatalog;
        public BattleRequest request;
    }

    private Deps _deps;
    private BattleSession _session;
    private BattleContext _context => _session.context;

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
        _session?.Dispose();
    }

    public void InitDependencies(Deps deps)
    {
        _deps = deps;
    }

    public void Initialize()
    {
        InitializeBattleSession();

        InitializeUIBattle();

        InitializePlayer();

        InitializeMap();
        
        _session.Start();
    }

    private void InitializeBattleSession()
    {
        _session = new BattleSession(new BattleSession.Deps
        {
            request = _deps.request,
            user = _deps.user,
            stageService = _deps.stageService,
            monsterCatalog = _deps.monsterCatalog,
            projectileCatalog = _deps.projectileCatalog,
            dropItemCatalog = _deps.dropItemCatalog,
            actorContainer = _actorContainer,
            playerTransform = _player.transform,
            getSpawnPosition = GetEnemySpawnPosition,
            worldParent = transform,
        });
    }

    private void InitializeUIBattle()
    {
        _uiBattlePresenter = new UIBattlePresenter(_uiBattle, new UIBattlePresenter.Deps
        {
            assetService = _deps.assetService,
            sceneLoader = _deps.sceneLoader,
            popupService = _deps.popupService,
            battleState = _context.state,
            battleReward = _context.reward,
            dash = _player.dash,
            onDashAction = _player.HandleDashAction,
            onJoystickAction = _player.HandleJoystickAction,
            mainCamera = _playerCamera.mainCamera
        });
        _uiBattlePresenter.Initialize();
    }

    private void InitializePlayer()
    {
        _player.InitDependencies(new Player.Deps
        {
            assetService = _deps.assetService,
            env = _context
        });

        var mercenary = _context.mercenary;
        _player.Init(mercenary);
        if (_player.moveBehaviour is DirectionMove direction)
        {
            direction.Setup(_player.inputSource, _map.GetBounds);
        }
        int nextId = _context.GetNextActorID(mercenary.Id);
        int sortingOrder = _context.GetNextOrderInLayer(mercenary.roleType);
        _player.Enter(nextId, sortingOrder, HandlePlayerDied);

        _playerCamera.Init(_uiBattle.GetTopUIHeight(), GetMapBounds);

        _player.onGoldCollected += _context.HandleGoldCollected;
    }

    private void InitializeMap()
    {
        _map.Init(_context.GetMapOriginal());
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

        _session.Lose();
    }
}