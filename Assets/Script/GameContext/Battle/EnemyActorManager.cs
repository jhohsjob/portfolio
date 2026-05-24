using System;
using System.Collections.Generic;
using UnityEngine;


public class EnemyActorManager : MonoBehaviour
{
    private Func<Vector3> _getRandomPos;
    private Transform _enemyTarget;
    private IActorSpawner _actorSpawner;

    private List<int> _spawnRoleId = new List<int>();
    private List<int> _spawnRoleRate = new List<int>();

    private List<Enemy> _enemyList = new List<Enemy>();

    private int _totalSpawnRate;

    private int _dieCount = 0;
    public int dieCount => _dieCount;

    public void Init(Func<Vector3> getRandomPos, Transform enemyTarget, IActorSpawner actorSpawner)
    {
        _getRandomPos = getRandomPos;
        _enemyTarget = enemyTarget;
        _actorSpawner = actorSpawner;
    }

    public void SetSpawnList(MapLevel mapLevel)
    {
        _spawnRoleId.Clear();
        _spawnRoleRate.Clear();

        _totalSpawnRate = 0;

        foreach (var spawnInfo in mapLevel.spawnInfos)
        {
            _spawnRoleId.Add(spawnInfo.roleId);
            _spawnRoleRate.Add(spawnInfo.spawnRate);

            _totalSpawnRate += spawnInfo.spawnRate;
        }
    }

    public void Spawn(Action<ActorBase> onDied)
    {
        var roleId = GetSpawnRoleId();
        var role = MonsterManager.instance.GetMonsterById(roleId);
        var position = _getRandomPos();

        var enemy = _actorSpawner.Spawn<Enemy, MonsterDefinition>(role, position);
        if (enemy.moveBehaviour is ChaseTargetMove chaseTarget)
        {
            chaseTarget.Setup(_enemyTarget, 0.5f);
        }
        enemy.onDied += onDied;

        _enemyList.Add(enemy);
    }

    private int GetSpawnRoleId()
    {
        var rnd = UnityEngine.Random.Range(0, _totalSpawnRate);
        var id = 0;

        for (int i = 0; i < _spawnRoleId.Count; i++)
        {
            if (_spawnRoleRate[i] > rnd)
            {
                id = _spawnRoleId[i];
                break;
            }

            rnd -= _spawnRoleRate[i];
        }

        return id;
    }

    public void Return(Enemy enemy)
    {
        if (_enemyList.Contains(enemy) == false)
        {
            return;
        }

        _enemyList.Remove(enemy);

        _dieCount++;

        EventHelper.Send(EventName.EnemyDieEnd, this, enemy);
    }

    public Transform GetNearestEnemy(Vector3 origin, Vector3 forward, float searchRadius, float searchAngle)
    {
        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (var actor in _enemyList)
        {
            if (actor == null || actor.state.HasState(ActorState.Die))
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
