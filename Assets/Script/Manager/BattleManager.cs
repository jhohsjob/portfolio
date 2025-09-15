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

    public BattleStatus battleStatus { get; private set; } = BattleStatus.None;
    
    public Vector3 mapSize { get; private set; }

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
        battleStatus = status;

        switch (battleStatus)
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

        EventHelper.Send(EventName.BattleStatus, this, battleStatus);
    }

    private void OnChangeMapSize(object sender, object data)
    {
        if (data == null || (data is Vector3) == false)
        {
            return;
        }

        mapSize = (Vector3)data;
    }
}
