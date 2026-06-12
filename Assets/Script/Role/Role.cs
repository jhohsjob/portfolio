using System;
using UnityEngine;
using UnityEngine.Localization.Settings;


public abstract class Role<TData> : RoleBase where TData : RoleDefinition
{
    protected TData _data;
    public TData data => _data;

    public RoleType roleType => _data.roleType;
    public override int id => _data.id;
    public virtual string localTable => LocalTable.None;
    public override string name => LocalizationSettings.StringDatabase.GetLocalizedString(localTable, _data.GetNameKey());
    public string description => LocalizationSettings.StringDatabase.GetLocalizedString(localTable, _data.GetDescKey());

    public override Type behaviourType => _data.behaviourType;

    public override Vector3 resourceOffset => _data.resourceOffset;

    public int atk => 0;
    public float maxHP => _data.maxHP;
    public float moveSpeed => _data.moveSpeed;

    public override GameObject original => _data.body;

    public Role(TData data)
    {
        _data = data.DeepCopy();
    }
}