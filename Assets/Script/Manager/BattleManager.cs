using System;
using UnityEngine;


public class BattleManagerInitData
{
    public MapInfoData mapInfoData;
}

public class BattleManager : MonoSingleton<BattleManager>
{
    [NonSerialized]
    public BattleScene battleScene;

    public ActorManager actorManager;
    public EnemyManager enemyManager;
    public DropItemManager dropItemManager;
    public MapLevelManager mapLevelManager;

    private BattleStatus _battleStatus { get; set; } = BattleStatus.None;
    
    public Bounds mapBounds { get; private set; }

    public bool debugEnemyPause { get; private set; }

    protected override void OnAwake()
    {
        actorManager = new GameObject("ActorManager").AddComponent<ActorManager>();
        enemyManager = new GameObject("EnemyManager").AddComponent<EnemyManager>();
        dropItemManager = new GameObject("DropItemManager").AddComponent<DropItemManager>();
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
        if (Input.GetKeyDown(KeyCode.P))
        {
            debugEnemyPause = !debugEnemyPause;
        }
    }

    protected override void OnCallDestroy()
    {
        EventHelper.RemoveEventListener(EventName.ChangeMapSize, OnChangeMapSize);

        StopAllCoroutines();
    }

    public void Init(BattleManagerInitData data)
    {
        actorManager.Init(this);
        enemyManager.Init(this);
        dropItemManager.Init(this);
        mapLevelManager.Init(this, data.mapInfoData);
    }

    public void SetBattleScene(BattleScene battleScene)
    {
        this.battleScene = battleScene;
    }

    public void SetBattleStatus(BattleStatus status)
    {
        _battleStatus = status;

        switch (_battleStatus)
        {
            case BattleStatus.Run:
                Time.timeScale = 1f;
                break;

            case BattleStatus.Pause:
                Time.timeScale = 0f;
                break;

            case BattleStatus.BattleOver:
                break;
        }

        EventHelper.Send(EventName.BattleStatus, this, _battleStatus);
    }

    public bool IsBattleRun()
    {
        return _battleStatus == BattleStatus.Run;
    }

    private void OnChangeMapSize(object sender, object data)
    {
        if (data == null || (data is Bounds bounds) == false)
        {
            return;
        }

        mapBounds = bounds;
    }
}
