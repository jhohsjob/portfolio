using System;
using UnityEngine.Localization.Settings;

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
    Nature,
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

[Flags]
public enum StatType : long
{
    None = 0,

    Offensive = 1L << 0,
    Defensive = 1L << 1,
    Utility = 1L << 2,

    Attack = Offensive | (1L << 8),
    CriticalChance = Offensive | (1L << 9),
    CriticalDamage = Offensive | (1L << 10),

    MaxHP = Defensive | (1L << 24),
    Defense = Defensive | (1L << 25),

    MoveSpeed = Utility | (1L << 40),
}

public enum MercenaryAcquireType
{
    None,
    Default,
    CurrencyGold,
}

public static class StatTypeExtensions
{
    public static string ToName(this StatType type)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("StatTable", type.ToString());
    }
}