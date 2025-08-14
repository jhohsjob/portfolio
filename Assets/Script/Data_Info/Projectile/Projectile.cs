public class Projectile : Role<ProjectileData>
{
    public float power => _data.power;

    public Projectile(ProjectileData data) : base(data) { }
}
