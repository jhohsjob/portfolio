using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class Actor<TRole, TData> : ActorBase where TRole : Role<TData> where TData : RoleData
{
    [SerializeField]
    private GameObject _point;

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

    public override GameObject point => _point;
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
    }

    private void SetID(int id)
    {
        ID = id;

        gameObject.name = _role.name + "_" + ID;
    }

    public virtual void Enter(object data = null)
    {
        SetID(BattleManager.instance.actorManager.GetNextID(_role.id));
    }

    public override void BeHit(float damage)
    {
        _hp?.AdjustHP(-damage, OnChangeHP);
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
    }
}
