using System;
using System.Threading;
using UnityEngine;


public interface IBattleEnvironment
{
    IBattleState state { get; }
    IActorSpawner spawner { get; }
    Projectile GetProjectile(int id);
    Transform GetNearestEnemy(Vector3 origin, Vector3 forward, float radius, float angle);
}

public sealed class BattleContext : IDisposable, IBattleEnvironment
{
    public sealed class Deps
    {
        public BattleRequest request;
        public User user;
        public StageService stageService;
        public MonsterManager monsterManager;
        public ProjectileManager projectileManager;
        public DropItemManager dropItemManager;
        public Transform actorContainer;
        public Transform playerTransform;
        public Func<Vector3> getSpawnPosition;
        public Transform worldParent;
    }


    private readonly Deps _deps;
    private readonly CancellationTokenSource _cts = new();
    private bool _disposed;

    public Stage stage => _deps.request.stage;
    public Mercenary mercenary => _deps.request.mercenary;
    public CancellationToken token => _cts.Token;

    public BattleController controller { get; }
    public BattleRewardRuntime reward { get; }
    public IBattleState state => controller;
    public IActorSpawner spawner => _world.actorSpawner;

    public Projectile GetProjectile(int id) => _deps.projectileManager.GetProjectileById(id);

    private readonly StageRunner _stageRunner;
    private readonly BattleFlow _flow;
    private readonly ActorWorld _world;

    public BattleContext(Deps deps)
    {
        _deps = deps;

        controller = new BattleController();
        reward = new BattleRewardRuntime();
        _stageRunner = new StageRunner();

        _flow = new BattleFlow(controller, _stageRunner, stage);
        _flow.onStageCleared += HandleStageCleared;

        _world = new GameObject("ActorWorld").AddComponent<ActorWorld>();
        _world.transform.SetParent(deps.worldParent);
        _world.Init(new ActorWorld.Deps
        {
            battleState = controller,
            stage = stage,
            monsterManager = deps.monsterManager,
            dropItemManager = deps.dropItemManager,
            actorContainer = deps.actorContainer,
            enemyTarget = deps.playerTransform,
            getSpawnPosition = deps.getSpawnPosition,
        });
    }

    public void Start() => _flow.Start(token);
    public void Lose() => _flow.Lose();

    public int GetNextActorID(int roleId) => _world.GetNextActorID(roleId);
    public int GetNextOrderInLayer(RoleType type) => _world.GetNextOrderInLayer(type);
    public Transform GetMapOriginal() => _world.GetMapOriginal();
    public Transform GetNearestEnemy(Vector3 o, Vector3 f, float r, float a) => _world.GetNearestEnemy(o, f, r, a);
    public void HandleGoldCollected(int gold) => reward.AddGold(gold);

    private void HandleStageCleared(int stageId)
    {
        int next = _deps.stageService.NextStageID(stageId);
        _deps.user.SetStage(next);
        // todo: 전투 결과 서버 전송
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

        _flow.onStageCleared -= HandleStageCleared;
        _stageRunner.Clear();
        reward.Clear();

        if (_world != null)
        {
            UnityEngine.Object.Destroy(_world.gameObject);
        }
    }
}