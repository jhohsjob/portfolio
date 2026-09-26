using System;
using UnityEngine;


public abstract class ActorBase : MonoBehaviour
{
    public int ID { get; protected set; }
    public int roleId { get; protected set; }
    public string roleName { get; protected set; }
    public Team team { get; protected set; }

    public ActorStat stat { get; protected set; }
    public virtual float moveSpeed => stat.utility.moveSpeed;
    public Vector3 dir { get; set; }
    public float distance { get; set; }
    public Vector2 extents => _view != null ? _view.extents : Vector2.zero;
    public Vector3 muzzlePos => _view != null ? _view.muzzlePos : Vector3.zero;
    public Vector3 muzzleDir => _view != null ? _view.muzzleDir : Vector3.zero;

    // public List<Skill> _skillList { get; set; }
    protected ActorStateModel _state;
    public ActorStateModel state => _state;
    protected HPController _hp;
    protected ActorView _view;

    protected virtual void Awake()
    {
        // _skillList = new List<Skill>();

        _state = new ActorStateModel();
        _hp = new HPController();
        _view = gameObject.AddComponent<ActorView>();
    }

    public virtual void Init(RoleBase role)
    {
        roleId = role.id;
        roleName = role.name;

        stat = new ActorStat(role.stat);
    }

    public abstract void Enter(int id, int sortingOrder, Action<ActorBase> onDied);
    public abstract void BeHit(float damage);
    public abstract void ResetCollider(float time = 0.5f);

    public abstract void SetMoving(bool isMoving);
    public abstract void SetLookDirection(Quaternion lookDirection);
    public abstract void SetFlip(Vector2 dir);
}
