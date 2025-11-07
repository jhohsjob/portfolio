using System;
using UnityEngine;


public abstract class ActorBase : MonoBehaviour
{
    public abstract Transform point { get; }
    public abstract int ID { get; protected set; }
    public abstract int roleId { get; }
    public Team team { get; protected set; }
    public abstract event Action<ActorBase> cbDie;

    public abstract HPController hp { get; }

    public abstract void InitBase<TData>(Role<TData> role) where TData : RoleData;
    public abstract void BeHit(float damage);
}
