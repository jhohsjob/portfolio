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
            Debug.Log(string.Format("IDGenerator.Next. 한계치에 도달. nextSeq : {0}, limit : {1}" , nextSeq, limitValue));
        }

        return nextSeq;
    }
}

public class RoleManager : MonoBehaviour
{
    private BattleManager _gameManager = null;

    /// <summary> key : role id </summary>
    private Dictionary<int, Queue<Role>> _pool = new Dictionary<int, Queue<Role>>();
    /// <summary> key : role id </summary>
    private Dictionary<int, GameObject> _container = new Dictionary<int, GameObject>();
    /// <summary> key : role id </summary>
    private Dictionary<int, IDGenerator> _idGenSet = new Dictionary<int, IDGenerator>();

    private Enemy _enemyPrefab;
    private Projectile _projectilePrefab;

    private void Awake()
    {
        _enemyPrefab = Resources.Load<Enemy>("Prefabs/Enemy");
        _projectilePrefab = Resources.Load<Projectile>("Prefabs/Projectile");
    }

    public void Init(BattleManager gameManager)
    {
        _gameManager = gameManager;
    }

    public void InitEnemy(int id)
    {
        var data = GameTable.GetEnemyData(id);
        if (data == null)
        {
            return;
        }

        if (_pool.ContainsKey(id) == true)
        {
            return;
        }

        PoolGenerate(data);
    }

    public void InitProjectile(int id)
    {
        var data = GameTable.GetProjectileData(id);
        if (data == null)
        {
            return;
        }

        if (_pool.ContainsKey(id) == true)
        {
            return;
        }

        PoolGenerate(data);
    }

    private void PoolGenerate(RoleData data)
    {
        var container = new GameObject(data.roleName + "Container");
        container.transform.SetParent(transform, false);
        _container.Add(data.id, container);

        _pool.Add(data.id, new Queue<Role>());

        _idGenSet.Add(data.id, new IDGenerator(data.id));

        AddPool(data);
    }
    
    private void AddPool(RoleData data)
    {
        Role rolePrefab = null;
        switch (data)
        {
            case EnemyData:
                rolePrefab = _enemyPrefab;
                break;

            case ProjectileData:
                rolePrefab = _projectilePrefab;
                break;

            default:
                Debug.Assert(true);
                break;
        }

        for (int i = 0; i < 10; i++)
        {
            var wait = Instantiate(rolePrefab, _container[data.id].transform);
            wait.Init(data);
            _pool[data.id].Enqueue(wait);
        }
    }

    public Role GetRole(RoleData data, Transform parent, Vector3 position)
    {
        if (_pool.ContainsKey(data.id) == false)
        {
            return null;
        }
        if (_pool[data.id].Count == 0)
        {
            AddPool(data);
        }

        var result = _pool[data.id].Dequeue();
        result.transform.SetParent(parent, false);
        result.transform.localPosition = position;

        return result;
    }

    public void Retrieve(Role role)
    {
        role.transform.SetParent(_container[role.roleId].transform, false);
        role.transform.localPosition = Vector3.zero;

        _pool[role.roleId].Enqueue(role);
    }

    public int GetNextID(int roleId)
    {
        return _idGenSet.ContainsKey(roleId) == true ? _idGenSet[roleId].Next() : 0;
    }
}
