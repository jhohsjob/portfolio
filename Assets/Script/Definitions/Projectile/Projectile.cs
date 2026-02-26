public class Projectile : Role<ProjectileData>
{
    public float power => _data.power;
    public float distance => _data.distance;
    public bool beHitDie => _data.beHitDie;
    public ProjectileMoveType moveType => _data.moveType;

    public Skill skill;

    public Projectile(ProjectileData data) : base(data) { }
}
