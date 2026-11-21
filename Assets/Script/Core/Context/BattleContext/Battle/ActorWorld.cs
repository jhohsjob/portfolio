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
        public MonsterCatalog monsterCatalog;
        public DropItemCatalog dropItemCatalog;
    }


    private Deps _deps;

    private ActorSpawner _actorSpawner;
    private EnemySpawner _enemySpawner;
    private DropItemSpawner _dropItemSpawner;
    private MapLevelManager _mapLevelManager;

    public IActorSpawner actorSpawner => _actorSpawner;

    private MapLevel _mapLevel;

    public void Init(Deps deps)
    {
        _deps = deps;

        _actorSpawner = new GameObject("ActorSpawner").AddComponent<ActorSpawner>();
        _enemySpawner = new GameObject("EnemySpawner").AddComponent<EnemySpawner>();
        _dropItemSpawner = new GameObject("DropItemSpawner").AddComponent<DropItemSpawner>();
        _mapLevelManager = new GameObject("MapLevelManager").AddComponent<MapLevelManager>();

        Bind();

        _actorSpawner.transform.SetParent(transform);
        _enemySpawner.transform.SetParent(transform);
        _dropItemSpawner.transform.SetParent(transform);
        _mapLevelManager.transform.SetParent(transform);

        _actorSpawner.Init(deps.actorContainer);
        _enemySpawner.Init(deps.getSpawnPosition, deps.enemyTarget, _actorSpawner, deps.monsterCatalog);
        _dropItemSpawner.Init(_actorSpawner, deps.dropItemCatalog);
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
        return _enemySpawner.GetNearestEnemy(origin, forward, searchRadius, searchAngle);
    }

    public int GetNextActorID(int roleId)
    {
        return _actorSpawner.GetNextID(roleId);
    }

    public int GetNextOrderInLayer(RoleType roleType)
    {
        return _actorSpawner.GetNextOrderInLayer(roleType);
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
            _enemySpawner.Spawn(HandleEnemyReturn);
        }
    }

    private void HandleLevelChanged(MapLevel level)
    {
        _mapLevel = level;

        _enemySpawner.SetSpawnList(level);
        _dropItemSpawner.SetSpawnList(level);
        Preload(level);
    }

    private void Preload(MapLevel mapLevel)
    {
        foreach (var spawnInfo in mapLevel.spawnInfos)
        {
            var monster = _deps.monsterCatalog.GetMonsterById(spawnInfo.roleId);
            _actorSpawner.InitPool(monster);

            foreach (var dropItemRoleId in spawnInfo.dropItemRoleIds)
            {
                var dropItem = _deps.dropItemCatalog.GetDropItemById(dropItemRoleId);
                _actorSpawner.InitPool(dropItem);
            }
        }
    }

    private void HandleEnemyReturn(ActorBase actorBase)
    {
        if (actorBase is not Enemy enemy)
        {
            return;
        }

        _dropItemSpawner.Spawn(_mapLevel.level, enemy, HandleDropItemReturn);

        _enemySpawner.Return(enemy);
    }

    private void HandleDropItemReturn(ActorBase actorBase)
    {
        if (actorBase is ActorDIGold gold)
        {
            _dropItemSpawner.Return(gold);
        }
        else if (actorBase is ActorDIElement element)
        {
            _dropItemSpawner.Return(element);
        }
    }
}