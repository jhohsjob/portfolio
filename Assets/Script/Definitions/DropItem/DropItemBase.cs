public interface IDropItemData
{
    DropItemType type { get; }
}

public abstract class DropItemBase<TData> : Role<TData> where TData : RoleDefinition, IDropItemData
{
    public DropItemType type => _data.type;

    protected DropItemBase(TData data) : base(data) { }
}

public class DIElement : DropItemBase<DIElementDefinition>
{
    public ElementType elementType => _data.elementType;

    public DIElement(DIElementDefinition data) : base(data) { }
}

public class DIGold : DropItemBase<DIGoldDefinition>
{
    public int gold => _data.gold;

    public DIGold(DIGoldDefinition data) : base(data) { }
}