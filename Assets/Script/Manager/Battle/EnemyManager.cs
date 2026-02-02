using System.Collections.Generic;
using UnityEngine;


public class EnemyManager : MonoBehaviour
{
    private BattleManager _gameManager = null;

    private List<int> _spawnRoleId = new List<int>();
    private List<int> _spawnRoleRate = new List<int>();

    private List<Enemy> _enemyList = new List<Enemy>();

    private int _mapLevel = 0;

    private int _dieCount = 0;
    public int dieCount => _dieCount;

    public void Init(BattleManager gameManager)
    {
        _gameManager = gameManager;

        _mapLevel = 0;
    }

    public void SetSpawnList(MapLevel mapLevel)
    {
        _mapLevel = mapLevel.level;

        _spawnRoleId?.Clear();
        _spawnRoleId.AddRange(mapLevel.spawnActorId);
        _spawnRoleRate?.Clear();
        _spawnRoleRate.AddRange(mapLevel.spawnActorRate);
    }

    public void Spawn()
    {
        var role = MonsterHander.instance.GetMonsterById(GetSpawnActorId());
        var parent = _gameManager.battleScene.actorContainer;
        var position = _gameManager.battleScene.GetRandomPos();
        var enemy = _gameManager.actorManager.GetActor(role, parent, position) as Enemy;
        enemy.Enter(_mapLevel);

        _enemyList.Add(enemy);

        EventHelper.Send(EventName.EnemySpawnEnd, this, enemy);
    }

    private int GetSpawnActorId()
    {
        var rnd = Random.Range(0, 100);
        var id = 0;

        for (int i = 0; i < _spawnRoleId.Count; i++)
        {
            if (_spawnRoleRate.Count <= i)
            {
                id = _spawnRoleId[i];
                break;
            }

            if (_spawnRoleRate[i] > rnd)
            {
                id = _spawnRoleId[i];
                break;
            }

            rnd -= _spawnRoleRate[i];
        }

        return id;
    }

    public void Die(Enemy enemy)
    {
        if (_enemyList.Contains(enemy) == false)
        {
            return;
        }

        _enemyList.Remove(enemy);

        _gameManager.actorManager.Return(enemy);

        _dieCount++;

        _gameManager.dropItemManager.Spawn(enemy);

        EventHelper.Send(EventName.EnemyDieEnd, this, enemy);
    }

    public Transform GetNearestEnemy(Vector3 origin, Vector3 forward, float searchRadius, float searchAngle)
    {
        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (var actor in _enemyList)
        {
            if (actor == null || actor.State.HasState(ActorState.Die))
            {
                continue;
            }

            Vector3 dir = actor.transform.position - origin;
            float dist = dir.sqrMagnitude;

            if (dist > searchRadius * searchRadius)
            {
                continue;
            }

            float angle = Vector3.Angle(forward, dir);
            if (angle > searchAngle * 0.5f)
            {
                continue;
            }

            if (dist < minDist)
            {
                minDist = dist;
                nearest = actor.transform;
            }
        }

        return nearest;
    }
}
