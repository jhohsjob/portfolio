public class Projectile : Role<ProjectileDefinition>
{
    public override string localTable => LocalTable.ProjectileTable;

    public float power => _data.power;
    public float distance => _data.distance;
    public bool beHitDie => _data.beHitDie;
    public ActorMoveType moveType => _data.moveType;

    public Skill skill;

    public Projectile(ProjectileDefinition data) : base(data) { }
}
