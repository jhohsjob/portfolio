public class Mercenary : Role<MercenaryData>
{
    public float maxHP => _data.maxHP;
    public SkillData skillData => _data.skillData;

    public Mercenary(MercenaryData data) : base(data) { }
}
