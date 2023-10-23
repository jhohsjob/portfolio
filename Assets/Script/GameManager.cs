using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoSingleton<GameManager>
{
    public GameScene gameScene;

    public Enums.GameStatus gameStatus { get; set; } = Enums.GameStatus.None;
    
    public Vector3 mapSize { get; private set; }

    [SerializeField]
    private float SPAWN_START_TIME = 3f;
    [SerializeField]
    private float SPAWN_TIME = 3f;
    private float _spawnTimer = 0f;
    private int _spawnNumber = 1;
    private int _spawnCount = 0;

    private Dictionary<int, GameLevelData> _gameLevelData = new Dictionary<int, GameLevelData>();

    private int _currentGameLevel = 1;
    
    protected override void OnAwake()
    {
        EventHelper.AddEventListener(EventName.GameStatus, OnGameStatus);
        EventHelper.AddEventListener(EventName.ChangeMapSize, OnChangeMapSize);

        var data = Resources.LoadAll("Data/GameLevel", typeof(ScriptableObject));
        foreach (GameLevelData item in data)
        {
            _gameLevelData.Add(item.level, item);
        }
    }

    void Update()
    {
        if (gameStatus == Enums.GameStatus.Run)
        {
            _spawnTimer += Time.deltaTime;

            if (_spawnTimer >= SPAWN_TIME)
            {
                _spawnTimer = 0f;
                _spawnCount++;

                Spawn();

                CheckNextLevel();
            }
        }
    }

    public void Init(GameScene gameScene)
    {
        this.gameScene = gameScene;
    }

    private IEnumerator SpawnStart()
    {
        yield return new WaitForSeconds(SPAWN_START_TIME);

        _spawnTimer = SPAWN_TIME;
    }

    private void Spawn()
    {
        var enemy = SpawnManager.instance.Spawn(Enums.SpawnType.Enemy, _spawnNumber, gameScene.enemyContainer);

        var x = Random.Range(-mapSize.x, mapSize.x);
        var z = Random.Range(-mapSize.z, mapSize.z);

        enemy.transform.localPosition = new Vector3(x, 0, z);

        EnemyManager.instance.Spawn(enemy.GetComponent<Enemy>());
    }

    private void CheckNextLevel()
    {
        if (_gameLevelData.ContainsKey(_currentGameLevel + 1) == false)
        {
            return;
        }

        if (_spawnCount >= _gameLevelData[_currentGameLevel + 1].needSpawnCount)
        {
            _currentGameLevel++;

            EventHelper.Send(EventName.GameLevelUp, this, _gameLevelData[_currentGameLevel]);
        }
    }

    private void OnGameStatus(object sender, object data)
    {
        if (data == null || (data is Enums.GameStatus) == false)
        {
            return;
        }

        gameStatus = (Enums.GameStatus)data;

        switch (gameStatus)
        {
            case Enums.GameStatus.Run:
                StartCoroutine(SpawnStart());
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
