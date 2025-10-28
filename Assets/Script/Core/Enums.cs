public enum BattleStatus
{
    None,
    Run,
    Pause,
    BattleOver
}

public enum RoleType
{
    Projectile = 1000,
    Item = 2000,
    Enemy = 3000,
    Player = 4000,
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

public enum ProjectileMoveType
{
    Straight,
    Orbit,
    Homing,
    GroundZone
}