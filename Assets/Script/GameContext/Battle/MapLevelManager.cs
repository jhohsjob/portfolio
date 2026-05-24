using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MapLevelManager : MonoBehaviour
{
    private IBattleState _battleState;

    private Dictionary<int, MapLevel> _mapLevels = new Dictionary<int, MapLevel>();

    private MapLevel _currentMapLevel = null;
    private float _spawnTime = 0f;

    private float _spawnTimer = 0f;
    private int _expCount = 0;

    private Transform _mapOriginal;
    public Transform mapOriginal => _mapOriginal;

    public event Action<int> OnSpawn;
    public event Action<MapLevel> OnLevelChanged;

    void Update()
    {
        if (_battleState != null && _battleState.IsRunning())
        {
            _spawnTimer += Time.deltaTime;

            if (_spawnTimer >= _spawnTime)
            {
                _spawnTimer = 0f;

                //for (int i = 0; i < _currentMapLevel.spawnCount; i++)
                //{
                //    _enemyManager.Spawn();
                //}

                OnSpawn?.Invoke(_currentMapLevel.spawnCount);

                CheckNextLevel();
            }
        }
    }

    public void Init(IBattleState battleState)
    {
        _battleState = battleState;

        MapDefinition map = GameSession.instance.currentStage.mapDefinition;

        var datas = map.levelDefinitions;
        for (int i = 0; i < datas.Count; i++)
        {
            var nextData = i < datas.Count - 1 ? datas[i + 1] : null;
            _mapLevels.Add(datas[i].level, new MapLevel(datas[i], nextData));
        }

        _mapOriginal = map.map;

        SetCurrentMap(_mapLevels.First().Value);
    }

    private void CheckNextLevel()
    {
        _expCount++;

        if (_currentMapLevel.levelupCount == -1)
        {
            return;
        }

        if (_expCount >= _currentMapLevel.levelupCount)
        {
            if (_mapLevels.TryGetValue(_currentMapLevel.nextLevel, out var next))
            {
                SetCurrentMap(next);

                // EventHelper.Send(EventName.MapLevelUp, this, _currentMapLevel);
            }
        }
    }

    private void SetCurrentMap(MapLevel mapLevel)
    {
        _currentMapLevel = mapLevel;
        _spawnTime = _currentMapLevel.respawnTime;

        OnLevelChanged?.Invoke(mapLevel);
    }
}