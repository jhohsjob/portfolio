using System;
using UnityEngine;


public abstract class Role<TDef> : RoleBase where TDef : RoleDefinition
{
    protected TDef _def;
    public TDef def => _def;

    protected RoleStat _stat;
    public override RoleStat stat => _stat;

    public RoleType roleType => _def.roleType;
    public virtual LocalizationTable localTable => LocalizationTable.None;
    public string description => LocalizationHelper.GetLocalizedString(localTable, _def.GetDescKey());

    public override int Id => _def.Id;
    public override string name => LocalizationHelper.GetLocalizedString(localTable, _def.GetNameKey());
    public override int level => 1;
    public override int grade => 0;
    public override Type behaviourType => _def.behaviourType;
    public override GameObject original => _def.body;
    public override Vector3 resourceOffset => _def.resourceOffset;
    public override ActorMoveType moveType => _def.moveType;


    public Role(TDef def)
    {
        _def = def.DeepCopy();
        _stat = new RoleStat(_def.statDefinition);
    }
}