using System;
using UnityEngine;
using UnityEngine.Localization.Settings;


public abstract class Role<TData> : RoleBase where TData : RoleDefinition
{
    protected TData _data;
    public TData data => _data;

    protected RoleStat _stat;
    public override RoleStat stat => _stat;

    public RoleType roleType => _data.roleType;
    public virtual string localTable => LocalTable.None;
    public string description => LocalizationSettings.StringDatabase.GetLocalizedString(localTable, _data.GetDescKey());

    public override int id => _data.id;
    public override string name => LocalizationSettings.StringDatabase.GetLocalizedString(localTable, _data.GetNameKey());
    public override int level => 1;
    public override int grade => 0;
    public override Type behaviourType => _data.behaviourType;
    public override GameObject original => _data.body;
    public override Vector3 resourceOffset => _data.resourceOffset;
    public override ActorMoveType moveType => _data.moveType;


    public Role(TData data)
    {
        _data = data.DeepCopy();
        _stat = new RoleStat(_data.statDefinition);
    }
}