using System.Collections.Generic;


public static class DropItemFactory
{
    public static RoleBase Create(IDropItemData data)
    {
        if (data is DIElementData diElementData)
        {
            return new DIElement(diElementData);
        }

        return null;
    }
}


public class DropItemHander : Singleton<DropItemHander>
{
    private readonly Dictionary<int, RoleBase> _dic = new();
    private readonly List<RoleBase> _list = new();
    public List<RoleBase> list => _list;

    public void Init(Dictionary<int, DropItemData> DropItemDatas)
    {
        foreach (var data in DropItemDatas)
        {
            _dic.Add(data.Key, DropItemFactory.Create(data.Value));
        }
        _list.AddRange(_dic.Values);
    }

    public T GetDropItemByIndex<T>(int index) where T : RoleBase
    {
        if (index < 0 || index >= _list.Count)
        {
            return null;
        }

        return _list[index] as T;
    }

    public T GetDropItemById<T>(int id) where T : RoleBase
    {
        if (_dic.ContainsKey(id) == false)
        {
            return null;
        }

        return _dic[id] as T;
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
