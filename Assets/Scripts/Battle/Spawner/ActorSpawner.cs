using System.Collections.Generic;
using UnityEngine;


public interface IActorSpawner
{
    void InitPool(RoleBase role);
    TActor Spawn<TActor, TDef>(Role<TDef> role, Vector3 position) where TActor : ActorBase where TDef : RoleDefinition;
}

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

public class ActorSpawner : MonoBehaviour, IActorSpawner
{
    private Transform _actorContainer;
    private ActorFactory _actorFactory = new();

    /// <summary> key : role id </summary>
    private readonly Dictionary<int, ComponentPool<ActorBase>> _pool = new();

    /// <summary> key : role id </summary>
    private Dictionary<int, GameObject> _container = new();
    /// <summary> key : role id </summary>
    private Dictionary<int, IDGenerator> _idGenSet = new();

    private int _orderInLayer;

    private void Awake()
    {
    }

    private void OnDestroy()
    {
        foreach (var pool in _pool.Values)
        {
            pool.Dispose();
        }
        _pool.Clear();
    }

    public void Init(Transform actorContainer)
    {
        _actorContainer = actorContainer;

        _orderInLayer = 10;
    }

    private void PoolGenerate(RoleBase role)
    {
        var container = new GameObject(role.name + "Container");
        container.transform.SetParent(transform, false);
        _container.Add(role.Id, container);
        _idGenSet.Add(role.Id, new IDGenerator(role.Id));

        _pool.Add(role.Id, new ComponentPool<ActorBase>(new ComponentPool<ActorBase>.Options
        {
            name = $"Actor:{role.name}",
            prewarm = 10,
            expandCount = 10,
            create = () => _actorFactory.CreateActor(role, container.transform),
            onRelease = actor =>
            {
                actor.transform.SetParent(container.transform, false);
                actor.transform.localPosition = Vector3.zero;
            }
        }));
    }

    private ActorBase GetActor<TDef>(Role<TDef> role, Transform parent, Vector3 position) where TDef : RoleDefinition
    {
        if (_pool.TryGetValue(role.Id, out var pool) == false)
        {
            return null;
        }
        
        var result = pool.Get();
        result.transform.SetParent(parent, false);
        result.transform.localPosition = position;
        return result;
    }

    public void InitPool(RoleBase role)
    {
        if (role == null || _pool.ContainsKey(role.Id))
        {
            return;
        }

        PoolGenerate(role);
    }

    public TActor Spawn<TActor, TDef>(Role<TDef> role, Vector3 pos) where TActor : ActorBase where TDef : RoleDefinition
    {
        int id = GetNextID(role.Id);
        int order = GetNextOrderInLayer(role.roleType);
        var actor = GetActor(role, _actorContainer, pos) as TActor;
        if (actor == null)
        {
            Debug.LogError($"[ActorManager] Spawn failed. roleId:{role.Id}, type:{typeof(TActor).Name} (pool not ready or type mismatch)");
            return null;
        }

        actor.onRelease = HandleActorDied;
        actor.Enter(id, order, null);

        return actor;
    }

    private void Return(ActorBase actor)
    {
        if (_pool.TryGetValue(actor.roleId, out var pool))
        {
            pool.Release(actor);   // 이중 반환이면 로그로 즉시 발견
        }
    }

    public int GetNextID(int roleId)
    {
        return _idGenSet.ContainsKey(roleId) == true ? _idGenSet[roleId].Next() : 0;
    }

    public int GetNextOrderInLayer(RoleType roleType = RoleType.None)
    {
        return (int)roleType + _orderInLayer++;
    }

    private void HandleActorDied(ActorBase actor)
    {
        Return(actor);
    }
}
