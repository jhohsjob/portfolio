using System.Collections.Generic;


public class ActorStat
{
    public OffensiveActorStat offensive { get; }
    public DefensiveActorStat defensive { get; }
    public UtilityActorStat utility { get; }

    public ActorStat(RoleStat roleStat)
    {
        offensive = new OffensiveActorStat();
        defensive = new DefensiveActorStat();
        utility = new UtilityActorStat();

        CopyRoleStat(roleStat);
    }

    private void CopyRoleStat(RoleStat roleStat)
    {
        // Offensive
        offensive.attack = roleStat.offensive.attack;

        offensive.criticalChance = roleStat.offensive.criticalChance;

        offensive.criticalDamage = roleStat.offensive.criticalDamage;


        // Defensive
        defensive.maxHp = roleStat.defensive.maxHp;

        defensive.defense = roleStat.defensive.defense;

        // Utility
        utility.moveSpeed = roleStat.utility.moveSpeed;
    }

    public float GetStat(StatType type)
    {
        if ((type & StatType.Offensive) != 0) return offensive.GetStat(type);
        if ((type & StatType.Defensive) != 0) return defensive.GetStat(type);
        if ((type & StatType.Utility) != 0) return utility.GetStat(type);

        return 0f;
    }
}

public class OffensiveActorStat : IStat
{
    private static readonly StatType[] _types =
    {
        StatType.Attack,
        StatType.CriticalChance,
        StatType.CriticalDamage
    };

    public float attack { get; internal set; }
    public float criticalChance { get; internal set; }
    public float criticalDamage { get; internal set; }
    
    public IReadOnlyList<StatType> GetAllType()
    {
        return _types;
    }

    public float GetStat(StatType type)
    {
        return type switch
        {
            StatType.Attack => attack,
            StatType.CriticalChance => criticalChance,
            StatType.CriticalDamage => criticalDamage,
            _ => 0f
        };
    }
}


public class DefensiveActorStat : IStat
{
    private static readonly StatType[] _types =
    {
        StatType.MaxHp,
        StatType.Defense
    };

    public float maxHp { get; internal set; }
    public float defense { get; internal set; }

    public IReadOnlyList<StatType> GetAllType()
    {
        return _types;
    }

    public float GetStat(StatType type)
    {
        return type switch
        {
            StatType.MaxHp => maxHp,
            StatType.Defense => defense,
            _ => 0f
        };
    }
}


public class UtilityActorStat : IStat
{
    private static readonly StatType[] _types =
    {
        StatType.MoveSpeed
    };

    public float moveSpeed { get; internal set; }

    public IReadOnlyList<StatType> GetAllType()
    {
        return _types;
    }

    public float GetStat(StatType type)
    {
        return type switch
        {
            StatType.MoveSpeed => moveSpeed,
            _ => 0f
        };
    }
}