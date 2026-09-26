using UnityEngine;


public static class DamageCalculator
{
    public static DamageResult Calculate(in DamageRequest request)
    {
        OffensiveActorStat offensive = request.attacker.offensive;

        DefensiveActorStat defensive = request.target.defensive;

        float damage = offensive.attack * request.multiplier;

        bool isCritical = Random.value * 100f < offensive.criticalChance;
        if (isCritical)
        {
            damage *= offensive.criticalDamage / 100f;
        }

        damage *= 100f / (100f + Mathf.Max(0f, defensive.defense));
        damage = Mathf.Max(1f, Mathf.Floor(damage));

        return new DamageResult(Mathf.Max(1f, damage), isCritical);
    }
}