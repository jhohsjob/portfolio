using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class Actor<TRole, TData> : ActorBase where TRole : Role<TData> where TData : RoleDefinition
{
    public override int ID { get; protected set; }

    protected TRole _role { get; private set; }
    public TRole role => _role;
    public override float moveSpeed => _role.moveSpeed;
    public override Vector3 dir { get; set; }
    public override float distance => 0f;

    protected ActorStateModel _state;
    public override ActorStateModel state => _state;

    protected HPController _hp;
    public override HPController hp => _hp;
    protected ActorView _view;
    public override Vector2 extents => _view != null ? _view.extents : Vector2.zero;
    public override Vector3 muzzlePos => _view != null ? _view.muzzlePos : Vector3.zero;
    public override Vector3 muzzleDir => _view != null ? _view.muzzleDir : Vector3.zero;

    public List<Skill> _skillList { get; set; }

    public override int roleId => _role.id;

    public event Action<ActorBase> onDied;

    protected IMoveBehaviour _moveBehaviour;
    public IMoveBehaviour moveBehaviour => _moveBehaviour;

    protected virtual void Awake()
    {
        _skillList = new List<Skill>();

        _state = new ActorStateModel();
        _hp = new HPController();
        _view = gameObject.AddComponent<ActorView>();
    }

    protected virtual void Update()
    {
        if (_state.HasState(ActorState.Move))
        {
            _moveBehaviour?.UpdateMove();
        }
    }

    public override void InitBase(RoleBase role)
    {
        Init(role as TRole);
    }

    public override void Enter(int id, int sortingOrder, Action<ActorBase> onDied)
    {
        Bind();

        SetID(id);
        
        this.onDied += onDied;

        _state.SetState(ActorState.Idle);
        _state.onStateChanged += OnStateChanged;

        if (_role.maxHP > 0f)
        {
            _hp.Enter(_role.maxHP);
            _hp.cbChange += OnChangeHP;
            EventHelper.Send(EventName.HpBarConnection, this);
        }

        _view.Enter(sortingOrder);
    }

    public override void BeHit(float damage)
    {
        _hp?.Damage(damage);

        _view.PlayFlash();
    }

    public virtual void Init(TRole role)
    {
        _role = role;

        _view.Initialize(this, role);

        _moveBehaviour = MoveBehaviourFactory.Create(_role.data.moveType);
        _moveBehaviour?.Init(this);
    }

    protected virtual void DieAfter()
    {
        if (_hp.maxHP > 0)
        {
            EventHelper.Send(EventName.HpBarDisconnection, this);
        }
    }

    protected virtual void Die()
    {
        Unbind();

        _moveBehaviour?.Clear();

        onDied?.Invoke(this);
        onDied = null;

        _state.Clear();
        _hp.Clear();

        SetID(0);

        _view.Die();
    }


    protected virtual void Bind()
    {
        _view.Bind();
        _view.onBodyTriggerEnter += OnBodyTriggerEnter;
    }

    protected virtual void Unbind()
    {
        _view.onBodyTriggerEnter -= OnBodyTriggerEnter;
        _view.Unbind();
    }


    private void SetID(int id)
    {
        ID = id;

        gameObject.name = _role.name + "_" + ID;
    }

    public override void ResetCollider(float time = 0.5f)
    {
        _view.ResetCollider(time);
    }

    public override void SetMoving(bool isMoving)
    {
        _view.MoveAnimation(isMoving);
    }

    public override void SetLookDirection(Quaternion lookDirection)
    {
        _view.SetLookDirection(lookDirection);
    }

    public override void SetFlip(Vector2 dir)
    {
        _view.SetFlip(dir);
    }

    public virtual void OnChangeHP(ChangeHPData data)
    {
        if (data.remainHP == 0)
        {
            _state.SetState(ActorState.Die);
        }
    }

    protected virtual void OnStateChanged(ActorState state)
    {
        if (state == ActorState.Die)
        {
            Die();
        }
    }

    protected virtual void OnBodyTriggerEnter(Body other) { }
}
