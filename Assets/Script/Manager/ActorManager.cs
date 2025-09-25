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

    private Dictionary<int, AssetLoadingTask> _tasks = new Dictionary<int, AssetLoadingTask>();

    private int _orderInLayer;

    private void Awake()
    {
    }

    public void Init(BattleManager gameManager)
    {
        _gameManager = gameManager;

        _orderInLayer = 10;
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
        var role = DropItemHander.instance.GetDropItemById(id);
        if (role == null || _pool.ContainsKey(id) == true)
        {
            return;
        }

        switch (role)
        {
            case DIElement:
                PoolGenerate(role as DIElement);
                break;

            case DIGold:
                PoolGenerate(role as DIGold);
                break;

            //default:
            //    PoolGenerate(role);
            //    break;
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
        switch (role)
        {
            case Monster:
                if (_tasks.ContainsKey(role.id) == true)
                {
                    var original = _tasks[role.id].GetAsset<GameObject>().GetComponent<ActorBase>();

                    InstantiateActor(role, original);
                }
                else
                {
                    Client.asset.LoadAsset<GameObject>("Enemy", (task) =>
                    {
                        _tasks.Add(role.id, task);

                        var original = task.GetAsset<GameObject>().GetComponent<ActorBase>();

                        InstantiateActor(role, original);
                    });
                }
                break;

            case Projectile:
                if (_tasks.ContainsKey(role.id) == true)
                {
                    var original = _tasks[role.id].GetAsset<GameObject>().GetComponent<ActorBase>();

                    InstantiateActor(role, original);
                }
                else
                {
                    Client.asset.LoadAsset<GameObject>("2DProjectile", (task) =>
                    {
                        _tasks.Add(role.id, task);

                        var original = task.GetAsset<GameObject>().GetComponent<ActorBase>();

                        InstantiateActor(role, original);
                    });
                }
                break;

            case DIElement:
                if (_tasks.ContainsKey(role.id) == true)
                {
                    var original = _tasks[role.id].GetAsset<GameObject>().GetComponent<ActorBase>();

                    InstantiateActor(role, original);
                }
                else
                {
                    Client.asset.LoadAsset<GameObject>("DIElement", (task) =>
                    {
                        _tasks.Add(role.id, task);

                        var original = task.GetAsset<GameObject>().GetComponent<ActorBase>();

                        InstantiateActor(role, original);
                    });
                }
                break;

            case DIGold:
                if (_tasks.ContainsKey(role.id) == true)
                {
                    var original = _tasks[role.id].GetAsset<GameObject>().GetComponent<ActorBase>();

                    InstantiateActor(role, original);
                }
                else
                {
                    Client.asset.LoadAsset<GameObject>("DIGold", (task) =>
                    {
                        _tasks.Add(role.id, task);

                        var original = task.GetAsset<GameObject>().GetComponent<ActorBase>();

                        InstantiateActor(role, original);
                    });
                }
                break;

            default:
                Debug.Assert(true);
                break;
        }
    }

    private void InstantiateActor<TData>(Role<TData> role, ActorBase original) where TData : RoleData
    {
        for (int i = 0; i < 10; i++)
        {
            var wait = Instantiate(original, _container[role.id].transform);
            wait.InitBase(role);

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

    public int GetNextOrderInLayer()
    {
        return _orderInLayer++;
    }
}
