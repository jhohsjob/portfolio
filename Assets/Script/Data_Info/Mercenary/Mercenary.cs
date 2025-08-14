public class Mercenary : Role<MercenaryData>
{
    public float maxHP => _data.maxHP;

    public Mercenary(MercenaryData data) : base(data) { }
}
