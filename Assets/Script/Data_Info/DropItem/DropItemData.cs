public class DropItemData : RoleData, IDropItemData
{
    public DropItemType type { get; set; }

    public override void Init(RoleData data)
    {
        base.Init(data);

        if (data is DropItemData dropItemData)
        {
            type = dropItemData.type;
        }
    }
}