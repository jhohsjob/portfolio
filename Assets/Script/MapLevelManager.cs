using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MapLevelManager : MonoBehaviour
{
    private GameManager _gameManager = null;

    public Dictionary<int, MapLevel> mapLevels = new Dictionary<int, MapLevel>();

    private MapLevel _currentMapLevel = null;
    private float _spawnTime = 0f;

    private float _spawnTimer = 0f;
    private int _expCount = 0;

    void Update()
    {
        if (_gameManager != null && _gameManager.gameStatus == GameStatus.Run)
        {
            _spawnTimer += Time.deltaTime;

            if (_spawnTimer >= _spawnTime)
            {
                _spawnTimer = 0f;
                
                for (int i = 0; i < _currentMapLevel.spawnCount; i++)
                {
                    _gameManager.enemyManager.Spawn(_currentMapLevel.GetSpawnActorId());
                }

                _gameManager.mapLevelManager.CheckNextLevel();
            }
        }
    }

    public void Init(GameManager gameManager)
    {
        _gameManager = gameManager;

        var datas = Resources.LoadAll<MapLevelData>("Data/MapLevel"); // , typeof(MapLevelData));
        for (int i = 0; i < datas.Length; i++)
        {
            var nextData = i < datas.Length - 1 ? datas[i + 1] : null;
            mapLevels.Add(datas[i].level, new MapLevel(datas[i], nextData));
        }

        _currentMapLevel = mapLevels.First().Value;
        _spawnTime = _currentMapLevel.respawnTime;
    }


    public void CheckNextLevel()
    {
        _expCount++;

        if (_currentMapLevel.levelupCount == -1)
        {
            return;
        }

        if (_expCount >= _currentMapLevel.levelupCount)
        {
            _currentMapLevel = mapLevels[_currentMapLevel.nextLevel];
            _spawnTime = _currentMapLevel.respawnTime;

            EventHelper.Send(EventName.MapLevelUp, this, _currentMapLevel);
        }
    }
}
