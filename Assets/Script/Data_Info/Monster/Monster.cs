public class Monster : Role<MonsterData>
{
    public float maxHP => _data.maxHP;

    public Monster(MonsterData data) : base(data) { }
}
