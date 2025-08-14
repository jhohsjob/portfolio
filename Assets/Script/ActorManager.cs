using System.Collections.Generic;
using UnityEngine;


public class IDGenerator
{
    public int headNumber { get; private set; }
    public int limitValue { get; private set; }
    public int baseValue { get; private set; }
    public int count { get; private set; }
    public int seqNow { get { return baseValue + count; } }

    public IDGenerator(int headNumber)
    {
        this.headNumber = headNumber;

        Reset();
    }

    public void Reset()
    {
        limitValue = (headNumber + 1) * 1000;
        baseValue = headNumber * 1000 + 100;
        this.count = 0;
    }

    public int Next()
    {
        count++;
        var nextSeq = seqNow;
        if (limitValue <= nextSeq)
        {
            Debug.Log(string.Format("IDGenerator.Next. 한계치에 도달. nextSeq : {0}, limit : {1}", nextSeq, limitValue));
        }

        return nextSeq;
    }
}

public class ActorManager : MonoBehaviour
{
    private BattleManager _gameManager = null;

    /// <summary> key : role id </summary>
    private Dictionary<int, Queue<ActorBase>> _pool = new Dictionary<int, Queue<ActorBase>>();
    /// <summary> key : role id </summary>
    private Dictionary<int, GameObject> _container = new Dictionary<int, GameObject>();
    /// <summary> key : role id </summary>
    private Dictionary<int, IDGenerator> _idGenSet = new Dictionary<int, IDGenerator>();

    private Enemy _enemyPrefab;
    private ActorProjectile _projectilePrefab;
    private ActorDIElement _diElementPrefab;

    private void Awake()
    {
        _enemyPrefab = Resources.Load<Enemy>("Prefabs/Enemy");
        _projectilePrefab = Resources.Load<ActorProjectile>("Prefabs/Projectile");
        _diElementPrefab = Resources.Load<ActorDIElement>("Prefabs/DIElement");
    }

    public void Init(BattleManager gameManager)
    {
        _gameManager = gameManager;
    }

    public void InitEnemy(int id)
    {
        var role = MonsterHander.instance.GetMonsterById(id);
        if (role == null || _pool.ContainsKey(id) == true)
        {
            return;
        }

        PoolGenerate(role);
    }

    public void InitProjectile(int id)
    {
        var role = ProjectileHander.instance.GetProjectileById(id);
        if (role == null || _pool.ContainsKey(id) == true)
        {
            return;
        }

        PoolGenerate(role);
    }

    public void InitDropItem(int id)
    {
        var role = DropItemHander.instance.GetDropItemById<DIElement>(id);
        if (role == null || _pool.ContainsKey(id) == true)
        {
            return;
        }

        switch (role.type)
        {
            case DropItemType.Element:
                PoolGenerate(role as DIElement);
                break;

            default:
                PoolGenerate(role);
                break;
        }
    }

    private void PoolGenerate<TData>(Role<TData> role) where TData : RoleData
    {
        var container = new GameObject(role.name + "Container");
        container.transform.SetParent(transform, false);
        _container.Add(role.id, container);

        _pool.Add(role.id, new Queue<ActorBase>());

        _idGenSet.Add(role.id, new IDGenerator(role.id));

        AddPool(role);
    }

    private void AddPool<TData>(Role<TData> role) where TData : RoleData
    {
        ActorBase actorPrefab = null;
        switch (role)
        {
            case Monster:
                actorPrefab = _enemyPrefab;
                break;

            case Projectile:
                actorPrefab = _projectilePrefab;
                break;

            case DIElement:
                actorPrefab = _diElementPrefab;
                break;

            default:
                Debug.Assert(true);
                break;
        }

        for (int i = 0; i < 10; i++)
        {
            var wait = Instantiate(actorPrefab, _container[role.id].transform);
            wait.InitBase(role.data);
            
            _pool[role.id].Enqueue(wait);
        }
    }

    public ActorBase GetActor<TData>(Role<TData> role, Transform parent, Vector3 position) where TData : RoleData
    {
        if (_pool.ContainsKey(role.id) == false)
        {
            return null;
        }
        if (_pool[role.id].Count == 0)
        {
            AddPool(role);
        }

        var result = _pool[role.id].Dequeue();
        result.transform.SetParent(parent, false);
        result.transform.localPosition = position;

        return result;
    }

    public void Return(ActorBase actor)
    {
        actor.transform.SetParent(_container[actor.roleId].transform, false);
        actor.transform.localPosition = Vector3.zero;

        _pool[actor.roleId].Enqueue(actor);
    }

    public int GetNextID(int roleId)
    {
        return _idGenSet.ContainsKey(roleId) == true ? _idGenSet[roleId].Next() : 0;
    }
}
