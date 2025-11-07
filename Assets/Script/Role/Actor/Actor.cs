using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Actor<TRole, TData> : ActorBase where TRole : Role<TData> where TData : RoleData
{
    [SerializeField]
    protected Transform _point;
    protected Body _body;
    protected FlashShader _flashShader;
    protected Animator _animator;
    protected Collider2D _collider;

    public override int ID { get; protected set; }

    protected TRole _role { get; private set; }
    public TRole role => _role;
    protected ActorStateController _state;
    public ActorStateController state => _state;

    protected HPController _hp;
    public override HPController hp => _hp;

    public List<Skill> _skillList { get; set; }

    public override Transform point => _point;
    public override int roleId => _role.id;

    protected Action<ActorBase> _cbDie;
    public override event Action<ActorBase> cbDie
    {
        add { _cbDie -= value; _cbDie += value; }
        remove { _cbDie -= value; }
    }

    protected virtual void Awake()
    {
        _skillList = new List<Skill>();

        _state = new ActorStateController();
        _hp = new HPController();
    }

    public override void InitBase<T>(Role<T> role)
    {
        Init(role as TRole);
    }

    public virtual void Init(TRole role)
    {
        _role = role;

        var body = Instantiate(role.original, transform);
        body.transform.localPosition = role.resourceOffset;

        body.AddComponent<RenderCheck>();
        _body = body.AddComponent<Body>();
        _body.Init(this);

        _flashShader = body.GetComponent<FlashShader>();
        _animator = body.GetComponent<Animator>();
        _collider = body.GetComponent<Collider2D>();
        if (_collider != null) _collider.enabled = false;
    }

    private void SetID(int id)
    {
        ID = id;

        gameObject.name = _role.name + "_" + ID;
    }

    public virtual void Enter(object data = null)
    {
        SetID(BattleManager.instance.actorManager.GetNextID(_role.id));
        
        _body.Enter(_role.roleType);

        _state.SetState(ActorState.Idle);
        _state.cbStateChanged += OnStateChanged;

        if (_role.maxHP > 0f)
        {
            _hp.Enter(_role.maxHP);
            _hp.cbChange += OnChangeHP;
            EventHelper.Send(EventName.HpBarConnection, this);
        }

        if (_collider != null) _collider.enabled = true;
    }

    public override void BeHit(float damage)
    {
        _hp?.Damage(damage);

        _flashShader?.FlashOnce();
    }

    public virtual void OnChangeHP(ChangeHPData data)
    {
        if (data.remainHP == 0)
        {
            _state.SetState(ActorState.Die);
        }
    }

    protected virtual void Move() { }

    public void ResetCollider(float time = 0.5f)
    {
        StartCoroutine(coResetCollider(time));
    }

    IEnumerator coResetCollider(float time)
    {
        if (_collider != null) _collider.enabled = false;
        yield return WaitCache.Get(time);
        if (_collider != null) _collider.enabled = true;
    }

    protected virtual void DieAfter()
    {
        if (_hp.maxHP > 0)
        {
            EventHelper.Send(EventName.HpBarDisconnection, this);
        }

        _cbDie?.Invoke(this);
    }

    protected virtual void Die()
    {
        _state.Clear();
        _hp.Clear();

        SetID(0);

        if (_collider != null) _collider.enabled = false;
    }

    protected virtual void OnStateChanged(ActorState state)
    {
        if (state == ActorState.Die)
        {
            Die();
        }
    }
}
