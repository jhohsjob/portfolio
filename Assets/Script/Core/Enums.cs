public enum BattleStatus
{
    None,
    Run,
    Pause,
    BattleOver
}

public enum RoleType
{
    Player = 10000, 
    Enemy = 20000, 
    Projectile = 30000
}

public enum Team
{
    None,
    Player,
    Enemy
}

public enum DropItemType
{
    None,
    Element,
    Gold,
}

public enum ElementType
{
    Water,
    Forest,
    Fire
}

[System.Flags]
public enum ActorState
{
    None = 0,
    Idle = 1 << 0,
    Move = 1 << 1,
    Dash = 1 << 2,
    Die = 1 << 3,
}
