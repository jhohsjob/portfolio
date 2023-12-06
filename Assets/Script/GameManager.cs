using System;
using UnityEngine;

public class GameManagerInitData
{
    public MapInfoData mapInfoData;
}

public class GameManager : MonoSingleton<GameManager>
{
    [NonSerialized]
    public GameScene gameScene;

    public MapLevelManager mapLevelManager;
    public RoleManager roleManager;
    public EnemyManager enemyManager;

    public GameStatus gameStatus { get; set; } = GameStatus.None;
    
    public Vector3 mapSize { get; private set; }

    protected override void OnAwake()
    {
        EventHelper.AddEventListener(EventName.GameStatus, OnGameStatus);
        EventHelper.AddEventListener(EventName.ChangeMapSize, OnChangeMapSize);

        mapLevelManager = new GameObject("MapLevelManager").AddComponent<MapLevelManager>();
        roleManager = new GameObject("RoleManager").AddComponent<RoleManager>();
        enemyManager = new GameObject("EnemyManager").AddComponent<EnemyManager>();

        mapLevelManager.transform.SetParent(transform);
        roleManager.transform.SetParent(transform);
        enemyManager.transform.SetParent(transform);

        transform.position = new Vector3(-1000f, -1000f, -1000f);
    }

    public void Init(GameManagerInitData data)
    {
        mapLevelManager.Init(this, data.mapInfoData);
        roleManager.Init(this);
        enemyManager.Init(this);
    }

    public void SetGameScene(GameScene gameScene)
    {
        this.gameScene = gameScene;
    }

    private void OnGameStatus(object sender, object data)
    {
        if (data == null || (data is GameStatus) == false)
        {
            return;
        }

        gameStatus = (GameStatus)data;

        switch (gameStatus)
        {
            case GameStatus.Run:
                break;

            case GameStatus.Pause: 
                break;

            case GameStatus.GameOver:
                break;
        }
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
