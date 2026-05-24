using System;
using UnityEngine;


public class BattleManagerContext
{
    public User user;
    public StageService stageService;
    public ICurrencyService currencyService;

    public Transform actorContainer;
    public Transform playerTransform;
    public Func<Vector3> getSpawnPosition;
}

public class BattleManager : MonoBehaviour
{
    private BattleManagerContext _context;

    private BattleController _battleController;
    private StageRunner _stageRunner;
    private BattleFlow _battleFlow;
    private BattleRewardRuntime _battleReward;

    private ActorWorld _actorWorld;

    private DebugBattleInput _debugBattleInput;

    public IBattleState battleState => _battleController;
    public IBattleController battleController => _battleController;

    public IActorSpawner actorSpawner => _actorWorld.actorSpawner;
    public BattleRewardRuntime battleReward => _battleReward;

    private void Awake()
    {
    }

    private void OnDestroy()
    {
    }

    public void Initialize(BattleManagerContext context)
    {
        _context = context;

        _battleController = new BattleController();
        _stageRunner = new StageRunner();
        _battleFlow = new BattleFlow(_battleController, _stageRunner, GameSession.instance.currentStage);
        _battleFlow.onStageCleared += HandleStageCleared;
        _battleReward = new BattleRewardRuntime();

        _actorWorld = new GameObject("ActorWorld").AddComponent<ActorWorld>();
        _actorWorld.transform.SetParent(transform);
        _actorWorld.Init(new ActorWorldContext
        {
            battleState = _battleController,

            actorContainer = _context.actorContainer,
            enemyTarget = _context.playerTransform,
            getSpawnPosition = _context.getSpawnPosition,
        });

        _debugBattleInput = new GameObject("DebugBattleInput").AddComponent<DebugBattleInput>();
        _debugBattleInput.transform.SetParent(transform);
    }

    public void StartBattle()
    {
        _battleFlow.Start();
    }

    public void LoseEndBattle()
    {
        _battleFlow.Lose();
    }

    public int GetNextActorID(int roleId)
    {
        return _actorWorld.GetNextActorID(roleId);
    }

    public int GetNextOrderInLayer(RoleType roleType)
    {
        return _actorWorld.GetNextOrderInLayer(roleType);
    }

    public Transform GetMapOriginal()
    {
        return _actorWorld.GetMapOriginal();
    }

    public Transform GetNearestEnemy(Vector3 origin, Vector3 forward, float searchRadius, float searchAngle)
    {
        return _actorWorld.GetNearestEnemy(origin, forward, searchRadius, searchAngle);
    }

    public void HandleGoldCollected(int gold)
    {
        _battleReward.AddGold(gold);
    }

    private void HandleStageCleared(int stageId)
    {
        int nextStageId = _context.stageService.NextStageID(stageId);

        _context.user.SetStage(nextStageId);

        _context.currencyService.Change(CurrencyType.Gold, _battleReward.gold);
    }
}