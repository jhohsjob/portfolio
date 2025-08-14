using System.Collections.Generic;

public class MonsterHander : Singleton<MonsterHander>
{
    private Dictionary<int, Monster> _dic = new Dictionary<int, Monster>();
    private List<Monster> _list = new List<Monster>();
    public List<Monster> list => _list;

    public void Init(Dictionary<int, MonsterData> MonsterDatas)
    {
        foreach (var data in MonsterDatas)
        {
            _dic.Add(data.Key, new Monster(data.Value));
        }
        _list.AddRange(_dic.Values);
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
