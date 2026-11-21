using System;
using System.Threading;
using UnityEngine;


public sealed class BattleSession : IDisposable
{
    public sealed class Deps
    {
        public BattleRequest request;
        public User user;
        public StageService stageService;
        public MonsterCatalog monsterCatalog;
        public ProjectileCatalog projectileCatalog;
        public DropItemCatalog dropItemCatalog;
        public Transform actorContainer;
        public Transform playerTransform;
        public Func<Vector3> getSpawnPosition;
        public Transform worldParent;
    }


    private readonly Deps _deps;
    private readonly CancellationTokenSource _cts = new();
    private readonly BattleState _state;
    private readonly StageRunner _stageRunner;
    private readonly BattleFlow _flow;
    private readonly ActorWorld _world;
    private bool _disposed;

    public BattleContext context { get; }
    public CancellationToken token => _cts.Token;

    public BattleSession(Deps deps)
    {
        _deps = deps;

        var stage = deps.request.stage;

        _state = new BattleState();
        _stageRunner = new StageRunner();

        _flow = new BattleFlow(_state, _stageRunner, stage);
        _flow.onStageCleared += HandleStageCleared;

        _world = new GameObject("ActorWorld").AddComponent<ActorWorld>();
        _world.transform.SetParent(deps.worldParent);
        _world.Init(new ActorWorld.Deps
        {
            battleState = _state,
            stage = stage,
            monsterCatalog = deps.monsterCatalog,
            dropItemCatalog = deps.dropItemCatalog,
            actorContainer = deps.actorContainer,
            enemyTarget = deps.playerTransform,
            getSpawnPosition = deps.getSpawnPosition,
        });

        context = new BattleContext(new BattleContext.Deps
        {
            request = deps.request,
            state = _state,
            reward = new BattleRewardRuntime(),
            world = _world,
            projectileCatalog = deps.projectileCatalog,
        });
    }

    public void Start() => _flow.Start(token);
    public void Lose() => _flow.Lose();

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
        context.reward.Clear();

        if (_world != null)
        {
            UnityEngine.Object.Destroy(_world.gameObject);
        }
    }
}