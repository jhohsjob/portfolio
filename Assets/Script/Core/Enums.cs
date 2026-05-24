public enum BattleStatus
{
    None,
    Ready,
    Running,
    Paused,
    Win,
    Lose
}

public enum RoleType
{
    None = 0,
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
    None,
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

public enum ActorMoveType
{
    None = 0,
    Straight,
    Orbit,
    Homing,
    GroundZone,
    ChaseTarget,
    Direction
}

public enum CurrencyType
{
    None,
    Free,
    Gold
}

public enum RewardType
{
    Gold,
    Mercenary,
}

public enum ShopLimitType
{
    Daily,
    Lifetime
}

public enum PurchaseFailReason
{
    None = 0,
    NotEnoughCurrency,
    PurchaseLimitExceeded,
    InvalidProduct,
    AlreadyOwned,
    ServerError,
}

public enum LobbyMenu
{
    Shop,
    Barrack,
    Battle,
    Temp01,
    Temp02,
}