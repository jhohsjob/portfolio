using System;
using UnityEngine;


public abstract class ActorBase : MonoBehaviour
{
    public abstract Transform point { get; }
    public abstract int ID { get; protected set; }
    public abstract int roleId { get; }
    public Team team { get; protected set; }

    public abstract Body body { get; }
    public abstract HPController hp { get; }
    public abstract Animator animator { get; }
    public abstract float moveSpeed { get; }
    public abstract Vector3 dir { get; set; }
    public abstract float distance { get; }
    public abstract ActorStateController state { get; }

    public abstract void InitBase(RoleBase role);
    public abstract void Enter(int id, int sortingOrder, Action<ActorBase> onDied);
    public abstract void BeHit(float damage);
    public abstract void ResetCollider(float time = 0.5f);
}
