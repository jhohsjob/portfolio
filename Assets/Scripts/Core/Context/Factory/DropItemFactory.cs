public static class DropItemFactory
{
    public static RoleBase Create(IDropItem data)
    {
        if (data is DIElementDefinition diElementData)
        {
            return new DropItemElement(diElementData);
        }

        if (data is DIGoldDefinition diGoldData)
        {
            return new DropItemGold(diGoldData);
        }

        return null;
    }
}