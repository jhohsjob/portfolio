public class Monster : Role<MonsterDefinition>
{
    public override LocalizationTable localTable => LocalizationTable.MonsterTable;

    public Monster(MonsterDefinition data) : base(data) { }
}
