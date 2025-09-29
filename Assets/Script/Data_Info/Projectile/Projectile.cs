public class Projectile : Role<ProjectileData>
{
    public float power => _data.power;
    public float distance => _data.distance;

    public Skill skill;

    public Projectile(ProjectileData data) : base(data) { }
}
