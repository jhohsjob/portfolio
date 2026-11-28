public class Projectile : Role<ProjectileDefinition>
{
    public override LocalizationTable localTable => LocalizationTable.ProjectileTable;

    public float power => _def.power;
    public float distance => _def.distance;
    public bool beHitDie => _def.beHitDie;

    public Projectile(ProjectileDefinition def) : base(def) { }
}
