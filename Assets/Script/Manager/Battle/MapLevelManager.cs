using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MapLevelManager : MonoBehaviour
{
    private BattleManager _gameManager = null;

    public Dictionary<int, MapLevel> mapLevels = new Dictionary<int, MapLevel>();

    private MapLevel _currentMapLevel = null;
    private float _spawnTime = 0f;

    private float _spawnTimer = 0f;
    private int _expCount = 0;

    private Transform _mapOriginal;
    public Transform mapOriginal => _mapOriginal;

    void Update()
    {
        if (_gameManager != null && _gameManager.IsBattleRun())
        {
            _spawnTimer += Time.deltaTime;

            if (_spawnTimer >= _spawnTime)
            {
                _spawnTimer = 0f;
                
                for (int i = 0; i < _currentMapLevel.spawnCount; i++)
                {
                    _gameManager.enemyManager.Spawn();
                }

                _gameManager.mapLevelManager.CheckNextLevel();
            }
        }
    }

    public void Init(BattleManager gameManager, MapInfoData mapInfoData)
    {
        _gameManager = gameManager;

        var datas = mapInfoData.levelDatas;
        for (int i = 0; i < datas.Count; i++)
        {
            var nextData = i < datas.Count - 1 ? datas[i + 1] : null;
            mapLevels.Add(datas[i].level, new MapLevel(datas[i], nextData));
        }

        _mapOriginal = mapInfoData.map;

        SetCurrentMap(mapLevels.First().Value);
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
            SetCurrentMap(mapLevels[_currentMapLevel.nextLevel]);

            // EventHelper.Send(EventName.MapLevelUp, this, _currentMapLevel);
        }
    }

    private void SetCurrentMap(MapLevel mapLevel)
    {
        _currentMapLevel = mapLevel;
        _spawnTime = _currentMapLevel.respawnTime;

        _gameManager.enemyManager.SetSpawnList(mapLevel);
        _gameManager.dropItemManager.SetDropList(mapLevel);
    }
}
