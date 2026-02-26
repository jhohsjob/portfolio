using System.Collections.Generic;


public class MercenaryManager : Singleton<MercenaryManager>
{
    /// <summary>
    /// key : mercenary Id
    /// value : mercenary data
    /// </summary>
    private Dictionary<int, Mercenary> _dic = new();
    private List<Mercenary> _list = new();
    public IReadOnlyList<Mercenary> list => _list;

    public void Init(Dictionary<int, MercenaryData> mercenaryDatas)
    {
        foreach (var data in mercenaryDatas)
        {
            _dic.Add(data.Key, new Mercenary(data.Value));
        }
        _list.AddRange(_dic.Values);
    }

    public Mercenary GetMercenaryByIndex(int index)
    {
        if (index < 0 || index >= _list.Count)
        {
            return null;
        }

        return _list[index];
    }

    public Mercenary GetMercenaryById(int id)
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
