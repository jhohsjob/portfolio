using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class Actor : Role
{
    [SerializeField]
    public GameObject point;

    protected HP _hp { get; set; }
    public float HP => _hp.currentHP;
    public float maxHP => _hp.maxHP;

    public List<Skill>  _skillList { get; set; }

    private Action<ChangeHPData> _cbChangeHP;
    public event Action<ChangeHPData> cbChangeHP
    {
        add { _cbChangeHP -= value; _cbChangeHP += value; }
        remove { _cbChangeHP -= value; }
    }

    protected virtual void Awake()
    {
        _hp = new HP();
        _skillList = new List<Skill>();
    }

    public override void Init(RoleData roleData)
    {
        var data = roleData as ActorData;
        if (data == null)
        {
            return;
        }

        base.Init(data);
    }

    public override void Enter()
    {
        base.Enter();

        var data = _roleData as ActorData;

        _hp.Init(data.maxHP);
    }

    public virtual void BeHit(float damage)
    {
        _hp.AdjustHP(-damage, OnChangeHP);
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
}
