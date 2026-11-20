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

        public MonsterManager monsterManager;
        public ProjectileManager projectileManager;
        public DropItemManager dropItemManager;
        public BattleRequest request;
    }

    private Deps _deps;
    private BattleContext _battleContext;

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
        _battleContext?.Dispose();
    }

    public void InitDependencies(Deps deps)
    {
        _deps = deps;
    }

    public void Initialize()
    {
        InitializeBattleContext();

        InitializeUIBattle();

        InitializePlayer();

        InitializeMap();
        
        _battleContext.Start();
    }

    private void InitializeBattleContext()
    {
        _battleContext = new BattleContext(new BattleContext.Deps
        {
            request = _deps.request,
            user = _deps.user,
            stageService = _deps.stageService,
            monsterManager = _deps.monsterManager,
            projectileManager = _deps.projectileManager,
            dropItemManager = _deps.dropItemManager,
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
            battleController = _battleContext.controller,
            battleReward = _battleContext.reward,
            dashController = _player.dash,
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
            env = _battleContext
        });

        var mercenary = _battleContext.mercenary;
        _player.Init(mercenary);
        if (_player.moveBehaviour is DirectionMove direction)
        {
            direction.Setup(_player.inputSource, _map.GetBounds);
        }
        int nextId = _battleContext.GetNextActorID(mercenary.id);
        int sortingOrder = _battleContext.GetNextOrderInLayer(mercenary.roleType);
        _player.Enter(nextId, sortingOrder, HandlePlayerDied);

        _playerCamera.Init(_uiBattle.GetTopUIHeight(), GetMapBounds);

        _player.onGoldCollected += _battleContext.HandleGoldCollected;
    }

    private void InitializeMap()
    {
        _map.Init(_battleContext.GetMapOriginal());
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

        _battleContext.Lose();
    }
}