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


public class DropItemManager : Singleton<DropItemManager>
{
    private RoleFactory _factory;

    private readonly Dictionary<int, RoleBase> _dic = new();
    private readonly List<RoleBase> _list = new();
    public List<RoleBase> list => _list;

    public void Init(RoleFactory factory)
    {
        _factory = factory;
    }

    public void Setup(Dictionary<int, DropItemDefinition> DropItemDefinitions)
    {
        foreach (var data in DropItemDefinitions)
        {
            _dic.Add(data.Key, _factory.Create(data.Value));
        }
        _list.AddRange(_dic.Values);
    }

    public RoleBase GetDropItemById(int id)
    {
        if (_dic.TryGetValue(id, out var role) == false)
        {
            return null;
        }

        return role;
    }

    public int CaclIndex(int index)
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
