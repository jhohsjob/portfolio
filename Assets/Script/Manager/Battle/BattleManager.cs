using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class BattleManager : MonoSingleton<BattleManager>
{
    [NonSerialized]
    public BattleScene battleScene;

    public ActorManager actorManager;
    public EnemyActorManager enemyManager;
    public DropItemActorManager dropItemManager;
    public MapLevelManager mapLevelManager;

    private BattleStatus _battleStatus { get; set; } = BattleStatus.None;
    
    public Bounds mapBounds { get; private set; }

    public bool debugEnemyPause { get; private set; }

    protected override void OnAwake()
    {
        actorManager = new GameObject("ActorManager").AddComponent<ActorManager>();
        enemyManager = new GameObject("EnemyManager").AddComponent<EnemyActorManager>();
        dropItemManager = new GameObject("DropItemManager").AddComponent<DropItemActorManager>();
        mapLevelManager = new GameObject("MapLevelManager").AddComponent<MapLevelManager>();

        actorManager.transform.SetParent(transform);
        enemyManager.transform.SetParent(transform);
        dropItemManager.transform.SetParent(transform);
        mapLevelManager.transform.SetParent(transform);

        transform.position = new Vector3(-1000f, -1000f, -1000f);

        EventHelper.AddEventListener(EventName.ChangeMapSize, OnChangeMapSize);
    }

    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            debugEnemyPause = !debugEnemyPause;
        }
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            EventHelper.Send(EventName.DebugStageClear, this);
        }
    }

    protected override void OnCallDestroy()
    {
        EventHelper.RemoveEventListener(EventName.ChangeMapSize, OnChangeMapSize);

        StopAllCoroutines();
    }

    public void Init(BattleScene battleScene)
    {
        this.battleScene = battleScene;

        actorManager.Init(this);
        enemyManager.Init(this);
        dropItemManager.Init(this);
        mapLevelManager.Init(this);
        InitStage();
    }

    private void InitStage()
    {
        Stage stage = GameSession.instance.currentStage;
        stage.Init();
        stage.cbStageCleared += OnStageCleared;
    }

    //public void SetBattleScene(BattleScene battleScene)
    //{
    //    this.battleScene = battleScene;
    //}

    public void SetBattleStatus(BattleStatus status)
    {
        _battleStatus = status;

        switch (_battleStatus)
        {
            case BattleStatus.Running:
                Time.timeScale = 1f;
                break;

            case BattleStatus.Paused:
                Time.timeScale = 0f;
                break;

            case BattleStatus.Win:
                break;

            case BattleStatus.Lose:
                break;
        }

        EventHelper.Send(EventName.BattleStatus, this, _battleStatus);
    }

    public bool IsBattleRun()
    {
        return _battleStatus == BattleStatus.Running;
    }

    private void OnChangeMapSize(object sender, object data)
    {
        if (data == null || (data is Bounds bounds) == false)
        {
            return;
        }

        mapBounds = bounds;
    }

    private void OnStageCleared()
    {
        SetBattleStatus(BattleStatus.Win);
    }
}
