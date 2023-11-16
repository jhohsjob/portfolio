using System;
using UnityEngine;


public class GameManager : MonoSingleton<GameManager>
{
    [NonSerialized]
    public GameScene gameScene;

    [SerializeField]
    public MapLevelManager mapLevelManager;
    [SerializeField]
    public RoleManager roleManager;
    [SerializeField]
    public EnemyManager enemyManager;

    public GameStatus gameStatus { get; set; } = GameStatus.None;
    
    public Vector3 mapSize { get; private set; }

    protected override void OnAwake()
    {
        EventHelper.AddEventListener(EventName.GameStatus, OnGameStatus);
        EventHelper.AddEventListener(EventName.ChangeMapSize, OnChangeMapSize);
    }

    public void Init(GameScene gameScene)
    {
        this.gameScene = gameScene;

        mapLevelManager.Init(this);
        roleManager.Init(this);
        enemyManager.Init(this);
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
