using UnityEngine;


public static class CombatPowerCalculator
{
    public static int Calculate(RoleStat stat)
    {
        float attack = stat.GetStat(StatType.Attack);
        float criticalChance = stat.GetStat(StatType.CriticalChance);
        float criticalDamage = stat.GetStat(StatType.CriticalDamage);

        float maxHP = stat.GetStat(StatType.MaxHP);
        float defense = stat.GetStat(StatType.Defense);

        float criticalMultiplier = 1f + (criticalChance / 100f) * (criticalDamage / 100f - 1f);

        float attackPower = attack * criticalMultiplier;

        float defensePower = maxHP / 10f + defense * 2f;

        return Mathf.Max(1, Mathf.FloorToInt(attackPower + defensePower));
    }
}