using System;
using UnityEngine;


// public abstract class Actor<TRole, TData> : ActorBase where TRole : Role<TData> where TData : RoleDefinition
public abstract class Actor<TRole> : ActorBase where TRole : RoleBase
{
    public event Action<ActorBase> onDied;

    protected IMoveBehaviour _moveBehaviour;
    public IMoveBehaviour moveBehaviour => _moveBehaviour;

    protected override void Awake()
    {
        base.Awake();
    }

    protected virtual void Update()
    {
        if (_state.HasState(ActorState.Move))
        {
            _moveBehaviour?.UpdateMove();
        }
    }

    public override void Enter(int id, int sortingOrder, Action<ActorBase> onDied)
    {
        Bind();

        SetID(id);
        
        this.onDied += onDied;

        _state.SetState(ActorState.Idle);
        _state.onStateChanged += OnStateChanged;

        if (stat.defensive.maxHP > 0f)
        {
            _hp.Enter(stat.defensive.maxHP);
            _hp.onChanged += OnHpChanged;
            // EventHelper.Send(EventName.HpBarConnection, this);
        }

        _view.Enter(sortingOrder);
    }

    public override void BeHit(float damage)
    {
        _hp?.Damage(damage);

        _view.PlayFlash();
    }

    public override void Init(RoleBase role)
    {
        base.Init(role);

        _view.Initialize(this, role);

        _moveBehaviour = MoveBehaviourFactory.Create(role.moveType);
        _moveBehaviour?.Init(this);
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
        _view.onVisibleChanged += OnVisibleChanged;
    }

    protected virtual void Unbind()
    {
        _view.onBodyTriggerEnter -= OnBodyTriggerEnter;
        _view.onVisibleChanged -= OnVisibleChanged;
        _view.Unbind();
    }


    private void SetID(int id)
    {
        ID = id;

        gameObject.name = roleName + "_" + ID;
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

    public virtual void OnHpChanged(ChangeHPData data)
    {
        _view.SetHp(data);
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

    private void OnVisibleChanged(bool isVisible)
    {
        if (isVisible == true)
        {
            var data = _hp.GetCurrentData();
            _view.SetHp(data);
        }
    }
}
