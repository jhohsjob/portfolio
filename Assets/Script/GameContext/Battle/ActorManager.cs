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

public class ActorManager : MonoBehaviour, IActorSpawner
{
    private Transform _actorContainer;
    private ActorFactory _actorFactory = null;

    /// <summary> key : role id </summary>
    private Dictionary<int, Queue<ActorBase>> _pool = new Dictionary<int, Queue<ActorBase>>();
    /// <summary> key : role id </summary>
    private Dictionary<int, GameObject> _container = new Dictionary<int, GameObject>();
    /// <summary> key : role id </summary>
    private Dictionary<int, IDGenerator> _idGenSet = new Dictionary<int, IDGenerator>();

    private int _orderInLayer;

    private void Awake()
    {
        _actorFactory = new ActorFactory();
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
        _container.Add(role.id, container);

        _pool.Add(role.id, new Queue<ActorBase>());

        _idGenSet.Add(role.id, new IDGenerator(role.id));

        AddPool(role);
    }

    private void AddPool(RoleBase role)
    {
        for (int i = 0; i < 10; i++)
        {
            var actor = _actorFactory.CreateActor(role, _container[role.id].transform);
            _pool[role.id].Enqueue(actor);
        }
    }

    private ActorBase GetActor<TDef>(Role<TDef> role, Transform parent, Vector3 position) where TDef : RoleDefinition
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

    public void InitPool(RoleBase role)
    {
        if (role == null || _pool.ContainsKey(role.id))
        {
            return;
        }

        PoolGenerate(role);
    }

    public TActor Spawn<TActor, TDef>(Role<TDef> role, Vector3 pos) where TActor : ActorBase where TDef : RoleDefinition
    {
        int id = GetNextID(role.id);
        int order = GetNextOrderInLayer(role.roleType);
        var actor = GetActor(role, _actorContainer, pos) as TActor;
        actor.Enter(id, order, HandleActorDied);

        return actor;
    }

    private void Return(ActorBase actor)
    {
        actor.transform.SetParent(_container[actor.roleId].transform, false);
        actor.transform.localPosition = Vector3.zero;

        _pool[actor.roleId].Enqueue(actor);
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
