using System;
using UnityEngine;


public abstract class ActorBase : MonoBehaviour
{
    public abstract GameObject point { get; }
    public abstract int roleId { get; }
    public Team team { get; protected set; }
    public abstract float HP { get; }
    public abstract float maxHP { get; }
    public abstract event Action<ChangeHPData> cbChangeHP;

    public abstract void InitBase<TData>(Role<TData> role) where TData : RoleData;
    public abstract void BeHit(float damage);
}
