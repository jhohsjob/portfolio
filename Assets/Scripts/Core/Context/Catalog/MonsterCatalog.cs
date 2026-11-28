using System;
using System.Collections.Generic;


public class MonsterCatalog
{
    private RoleFactory _factory;

    private Dictionary<int, Monster> _dic = new();
    private List<Monster> _list = new();
    public IReadOnlyList<Monster> list => _list;

    public MonsterCatalog(RoleFactory factory)
    {
        _factory = factory;
    }

    public void Setup(Dictionary<int, MonsterDefinition> monsterDefinitions)
    {
        if (_factory == null)
        {
            throw new InvalidOperationException("MonsterManager.Init must be called before Setup");
        }

        var dic = new Dictionary<int, Monster>(monsterDefinitions.Count);
        foreach (var data in monsterDefinitions)
        {
            dic.Add(data.Key, (Monster)_factory.Create(data.Value));
        }

        _dic = dic;
        _list = new List<Monster>(dic.Values);
    }

    public Monster GetMonsterByIndex(int index)
    {
        if (index < 0 || index >= _list.Count)
        {
            return null;
        }

        return _list[index];
    }

    public Monster GetMonsterById(int id)
    {
        if (_dic.ContainsKey(id) == false)
        {
            return null;
        }

        return _dic[id];
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
