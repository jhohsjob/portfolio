public readonly struct DamageRequest
{
    public readonly ActorStat attacker;
    public readonly ActorStat target;
    public readonly float multiplier;

    public DamageRequest(ActorStat attacker, ActorStat target, float multiplier)
    {
        this.attacker = attacker;
        this.target = target;
        this.multiplier = multiplier;
    }
}