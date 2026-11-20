using System;
using UnityEngine;


public class ActorWorld : MonoBehaviour
{
    public sealed class Deps
    {
        public IBattleState battleState;

        public Transform actorContainer;
        public Transform enemyTarget;
        public Func<Vector3> getSpawnPosition;

        public Stage stage;
        public MonsterManager monsterManager;
        public DropItemManager dropItemManager;
    }


    private Deps _deps;

    private ActorManager _actorManager;
    private EnemyActorManager _enemyManager;
    private DropItemActorManager _dropItemManager;
    private MapLevelManager _mapLevelManager;

    public IActorSpawner actorSpawner => _actorManager;

    private MapLevel _mapLevel;

    public void Init(Deps deps)
    {
        _deps = deps;

        _actorManager = new GameObject("ActorManager").AddComponent<ActorManager>();
        _enemyManager = new GameObject("EnemyManager").AddComponent<EnemyActorManager>();
        _dropItemManager = new GameObject("DropItemManager").AddComponent<DropItemActorManager>();
        _mapLevelManager = new GameObject("MapLevelManager").AddComponent<MapLevelManager>();

        Bind();

        _actorManager.transform.SetParent(transform);
        _enemyManager.transform.SetParent(transform);
        _dropItemManager.transform.SetParent(transform);
        _mapLevelManager.transform.SetParent(transform);

        _actorManager.Init(deps.actorContainer);
        _enemyManager.Init(deps.getSpawnPosition, deps.enemyTarget, _actorManager, deps.monsterManager);
        _dropItemManager.Init(_actorManager, deps.dropItemManager);
        _mapLevelManager.Init(deps.battleState, deps.stage);

        transform.position = new Vector3(-1000f, -1000f, -1000f);
    }

    private void Bind()
    {
        _mapLevelManager.OnSpawn += HandleSpawn;
        _mapLevelManager.OnLevelChanged += HandleLevelChanged;
    }

    public Transform GetNearestEnemy(Vector3 origin, Vector3 forward, float searchRadius, float searchAngle)
    {
        return _enemyManager.GetNearestEnemy(origin, forward, searchRadius, searchAngle);
    }

    public int GetNextActorID(int roleId)
    {
        return _actorManager.GetNextID(roleId);
    }

    public int GetNextOrderInLayer(RoleType roleType)
    {
        return _actorManager.GetNextOrderInLayer(roleType);
    }

    public Transform GetMapOriginal()
    {
        return _mapLevelManager.mapOriginal;
    }

    private void HandleSpawn(int count)
    {
        if (_deps.battleState.IsRunning() == false)
        {
            return;
        }

        for (int i = 0; i < count; i++)
        {
            _enemyManager.Spawn(HandleEnemyReturn);
        }
    }

    private void HandleLevelChanged(MapLevel level)
    {
        _mapLevel = level;

        _enemyManager.SetSpawnList(level);
        _dropItemManager.SetSpawnList(level);
        Preload(level);
    }

    private void Preload(MapLevel mapLevel)
    {
        foreach (var spawnInfo in mapLevel.spawnInfos)
        {
            var monster = _deps.monsterManager.GetMonsterById(spawnInfo.roleId);
            _actorManager.InitPool(monster);

            foreach (var dropItemRoleId in spawnInfo.dropItemRoleIds)
            {
                var dropItem = _deps.dropItemManager.GetDropItemById(dropItemRoleId);
                _actorManager.InitPool(dropItem);
            }
        }
    }

    private void HandleEnemyReturn(ActorBase actorBase)
    {
        if (actorBase is not Enemy enemy)
        {
            return;
        }

        _dropItemManager.Spawn(_mapLevel.level, enemy, HandleDropItemReturn);

        _enemyManager.Return(enemy);
    }

    private void HandleDropItemReturn(ActorBase actorBase)
    {
        if (actorBase is ActorDIGold gold)
        {
            _dropItemManager.Return(gold);
        }
        else if (actorBase is ActorDIElement element)
        {
            _dropItemManager.Return(element);
        }
    }
}