public interface IDropItemData
{
    DropItemType type { get; }
}

public abstract class DropItemBase<TData> : Role<TData> where TData : RoleData, IDropItemData
{
    public DropItemType type => _data.type;

    protected DropItemBase(TData data) : base(data) { }
}

public class DIElement : DropItemBase<DIElementData>
{
    public ElementType elementType => _data.elementType;

    public DIElement(DIElementData data) : base(data) { }
}