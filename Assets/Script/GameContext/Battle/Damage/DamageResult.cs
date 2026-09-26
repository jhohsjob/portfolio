public readonly struct DamageResult
{
    public readonly float damage;
    public readonly bool isCritical;

    public DamageResult(float damage, bool isCritical)
    {
        this.damage = damage;
        this.isCritical = isCritical;
    }
}