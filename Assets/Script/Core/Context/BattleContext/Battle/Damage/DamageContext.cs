public readonly struct DamageContext
{
    public readonly float attack;
    public readonly float damageMultiplier;
    public readonly float criticalChance;
    public readonly float criticalDamage;
    public readonly float defense;

    public DamageContext(
        float attack,
        float damageMultiplier,
        float criticalChance,
        float criticalDamage,
        float defense)
    {
        this.attack = attack;
        this.damageMultiplier = damageMultiplier;
        this.criticalChance = criticalChance;
        this.criticalDamage = criticalDamage;
        this.defense = defense;
    }
}