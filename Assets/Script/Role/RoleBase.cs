using System;
using UnityEngine;


public abstract class RoleBase
{
    public abstract int id { get; }
    public abstract string name { get; }
    public abstract int level { get; }
    public abstract int grade { get; }
    public abstract RoleStat stat { get; }
    public abstract Type behaviourType { get; }
    public abstract GameObject original { get; }
    public abstract Vector3 resourceOffset { get; }
    public abstract ActorMoveType moveType { get; }
}
