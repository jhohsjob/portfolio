public class Monster : Role<MonsterDefinition>
{
    public override string localTable => LocalTable.MonsterTable;

    public Monster(MonsterDefinition data) : base(data) { }
}
