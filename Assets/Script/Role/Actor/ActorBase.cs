using System;
using UnityEngine;


public abstract class ActorBase : MonoBehaviour
{
    public abstract int ID { get; protected set; }
    public abstract int roleId { get; }
    public Team team { get; protected set; }

    public abstract HPController hp { get; }
    public abstract float moveSpeed { get; }
    public abstract Vector3 dir { get; set; }
    public abstract float distance { get; }
    public abstract ActorStateModel state { get; }
    public abstract Vector2 extents { get; }
    public abstract Vector3 muzzlePos { get; }
    public abstract Vector3 muzzleDir { get; }

    public abstract void InitBase(RoleBase role);
    public abstract void Enter(int id, int sortingOrder, Action<ActorBase> onDied);
    public abstract void BeHit(float damage);
    public abstract void ResetCollider(float time = 0.5f);

    public abstract void SetMoving(bool isMoving);
    public abstract void SetLookDirection(Quaternion lookDirection);
    public abstract void SetFlip(Vector2 dir);
    // todo : delete
    //public virtual Transform point { get; }
    //public virtual Body body { get; }
    //public virtual Animator animator { get; }
}
