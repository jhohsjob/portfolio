using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class Actor<TRole, TData> : ActorBase where TRole : Role<TData> where TData : RoleData
{
    [SerializeField]
    protected Transform _point;
    protected Body _body;
    protected SpriteRenderer _sprite;
    protected FlashShader _flashShader;
    protected Animator _animator;
    protected BoxCollider2D _collider;

    public int ID { get; private set; }

    protected TRole _role { get; private set; }

    protected HP _hp { get; set; }

    protected Action<ChangeHPData> _cbChangeHP;
    public override event Action<ChangeHPData> cbChangeHP
    {
        add { _cbChangeHP -= value; _cbChangeHP += value; }
        remove { _cbChangeHP -= value; }
    }

    public List<Skill> _skillList { get; set; }

    public override Transform point => _point;
    public override int roleId => _role.id;
    public override float HP => _hp.currentHP;
    public override float maxHP => _hp.maxHP;

    protected virtual void Awake()
    {
        _hp = new HP();
        _skillList = new List<Skill>();
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

        _sprite = body.GetComponent<SpriteRenderer>();
        _flashShader = body.GetComponent<FlashShader>();
        _animator = body.GetComponent<Animator>();
        _collider = body.GetComponent<BoxCollider2D>();
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

        if (_sprite != null)
        {
            _sprite.sortingOrder = BattleManager.instance.actorManager.GetNextOrderInLayer();
        }

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

    protected virtual void Die()
    {
        SetID(0);

        if (_collider != null) _collider.enabled = false;
    }
}
