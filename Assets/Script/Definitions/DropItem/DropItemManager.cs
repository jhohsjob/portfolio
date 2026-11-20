using System.Collections.Generic;


public static class DropItemFactory
{
    public static RoleBase Create(IDropItemData data)
    {
        if (data is DIElementDefinition diElementData)
        {
            return new DIElement(diElementData);
        }

        if (data is DIGoldDefinition diGoldData)
        {
            return new DIGold(diGoldData);
        }

        return null;
    }
}


public class DropItemManager
{
    private RoleFactory _factory;

    private Dictionary<int, RoleBase> _dic = new();
    private List<RoleBase> _list = new();
    public IReadOnlyList<RoleBase> list => _list;

    public DropItemManager(RoleFactory factory)
    {
        _factory = factory;
    }

    public void Setup(Dictionary<int, DropItemDefinition> dropItemDefinitions)
    {
        if (_factory == null)
        {
            throw new System.InvalidOperationException("DropItemManager.Init must be called before Setup");
        }

        var dic = new Dictionary<int, RoleBase>(dropItemDefinitions.Count);
        foreach (var data in dropItemDefinitions)
        {
            dic.Add(data.Key, _factory.Create(data.Value));
        }

        _dic = dic;
        _list = new List<RoleBase>(dic.Values);
    }

    public RoleBase GetDropItemById(int id)
    {
        if (_dic.TryGetValue(id, out var role) == false)
        {
            return null;
        }

        return role;
    }

    public int CalcIndex(int index)
    {
        if (index < 0)
        {
            index = _list.Count - 1;
        }
        if (index >= _list.Count)
        {
            index = 0;
        }

        return index;
    }
}
