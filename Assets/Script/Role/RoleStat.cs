using System.Collections.Generic;
using UnityEngine;


public interface IStat
{
    IReadOnlyList<StatType> GetAllType();
    float GetStat(StatType type);
}

public class RoleStat
{
    private readonly StatDefinition _definition;

    public int level { get; private set; }

    public OffensiveRoleStat offensive { get; }
    public DefensiveRoleStat defensive { get; }
    public UtilityRoleStat utility { get; }

    public int combatPower => CombatPowerCalculator.Calculate(this);

    public RoleStat(StatDefinition definition, int level = 1)
    {
        _definition = definition;

        offensive = new OffensiveRoleStat();
        defensive = new DefensiveRoleStat();
        utility = new UtilityRoleStat();

        SetLevel(level);
    }

    public void SetLevel(int level)
    {
        this.level = Mathf.Max(1, level);

        float growth = this.level - 1;

        // Offensive
        offensive.attack = _definition.offensive.attack.value + _definition.offensive.attack.growth * growth;

        offensive.criticalChance = _definition.offensive.criticalChance.value + _definition.offensive.criticalChance.growth * growth;

        offensive.criticalDamage = _definition.offensive.criticalDamage.value + _definition.offensive.criticalDamage.growth * growth;


        // Defensive
        defensive.maxHP = _definition.defensive.maxHP.value + _definition.defensive.maxHP.growth * growth;

        defensive.defense = _definition.defensive.defense.value + _definition.defensive.defense.growth * growth;

        // Utility
        utility.moveSpeed = _definition.utility.moveSpeed;
    }

    public float GetStat(StatType type)
    {
        if ((type & StatType.Offensive) != 0) return offensive.GetStat(type);
        if ((type & StatType.Defensive) != 0) return defensive.GetStat(type);
        if ((type & StatType.Utility) != 0) return utility.GetStat(type);

        return 0f;
    }
}

public class OffensiveRoleStat : IStat
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


public class DefensiveRoleStat : IStat
{
    private static readonly StatType[] _types =
    {
        StatType.MaxHP,
        StatType.Defense
    };

    public float maxHP { get; internal set; }
    public float defense { get; internal set; }

    public IReadOnlyList<StatType> GetAllType()
    {
        return _types;
    }

    public float GetStat(StatType type)
    {
        return type switch
        {
            StatType.MaxHP => maxHP,
            StatType.Defense => defense,
            _ => 0f
        };
    }
}


public class UtilityRoleStat : IStat
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