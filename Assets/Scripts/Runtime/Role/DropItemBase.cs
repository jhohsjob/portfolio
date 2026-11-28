public interface IDropItem
{
    DropItemType type { get; }
}

public abstract class DropItemBase<TDef> : Role<TDef> where TDef : RoleDefinition, IDropItem
{
    public override LocalizationTable localTable => LocalizationTable.DropItemTable;

    public DropItemType type => _def.type;

    protected DropItemBase(TDef def) : base(def) { }
}

public class DropItemElement : DropItemBase<DIElementDefinition>
{
    public ElementType elementType => _def.elementType;

    public DropItemElement(DIElementDefinition def) : base(def) { }
}

public class DropItemGold : DropItemBase<DIGoldDefinition>
{
    public int goldAmount => _def.gold;

    public DropItemGold(DIGoldDefinition def) : base(def) { }
}