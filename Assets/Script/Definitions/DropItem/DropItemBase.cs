public interface IDropItemData
{
    DropItemType type { get; }
}

public abstract class DropItemBase<TData> : Role<TData> where TData : RoleDefinition, IDropItemData
{
    public override LocalizationTable localTable => LocalizationTable.DropItemTable;

    public DropItemType type => _data.type;

    protected DropItemBase(TData data) : base(data) { }
}

public class DropItemElement : DropItemBase<DIElementDefinition>
{
    public ElementType elementType => _data.elementType;

    public DropItemElement(DIElementDefinition data) : base(data) { }
}

public class DropItemGold : DropItemBase<DIGoldDefinition>
{
    public int goldAmount => _data.gold;

    public DropItemGold(DIGoldDefinition data) : base(data) { }
}