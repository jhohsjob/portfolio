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

    public int ID { get; private set; }

    protected TRole _role { get; private set; }
    public TRole role => _role;
    protected ActorStateController _state;
    public ActorStateController state => _state;

    protected HP _hp { get; set; }

    public List<Skill> _skillList { get; set; }

    public override Transform point => _point;
    public override int roleId => _role.id;
    public override float HP => _hp.currentHP;
    public override float maxHP => _hp.maxHP;

    protected Action<ChangeHPData> _cbChangeHP;
    public override event Action<ChangeHPData> cbChangeHP
    {
        add { _cbChangeHP -= value; _cbChangeHP += value; }
        remove { _cbChangeHP -= value; }
    }

    protected Action<ActorBase> _cbDie;
    public override event Action<ActorBase> cbDie
    {
        add { _cbDie -= value; _cbDie += value; }
        remove { _cbDie -= value; }
    }

    protected virtual void Awake()
    {
        _hp = new HP();
        _skillList = new List<Skill>();

        _state = new ActorStateController();
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
        _state.Clear();
        _state.SetState(ActorState.Idle);
        _state.OnStateChanged += OnStateChanged;

        SetID(BattleManager.instance.actorManager.GetNextID(_role.id));

        _body.Enter(_role.roleType);

        if (_collider != null) _collider.enabled = true;
    }

    public override void BeHit(float damage)
    {
        _hp?.AdjustHP(-damage, OnChangeHP);

        _flashShader?.FlashOnce();
    }

    public virtual void OnChangeHP(ChangeHPData data)
    {
        if (data.remainHP == 0)
        {
            Die();
        }
        else
        {
            _cbChangeHP?.Invoke(data);
        }
    }

    protected virtual void Move() { }

    public void ResetCollider()
    {
        StartCoroutine(coResetCollider());
    }

    IEnumerator coResetCollider()
    {
        if (_collider != null) _collider.enabled = false;
        yield return null;
        if (_collider != null) _collider.enabled = true;
    }

    protected virtual void DieAfter()
    {
        _cbDie?.Invoke(this);
    }

    protected virtual void Die()
    {
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
