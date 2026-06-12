using System;
using UnityEngine;

public abstract class RoleBase
{
    public abstract string name { get; }
    public abstract int id { get; }
    public abstract Type behaviourType { get; }
    public abstract GameObject original { get; }
    public abstract Vector3 resourceOffset { get; }
}
