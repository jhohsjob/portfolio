using System;

public abstract class RoleBase
{
    public abstract string name { get; }
    public abstract int id { get; }
    public abstract Type behaviourType { get; }
}
