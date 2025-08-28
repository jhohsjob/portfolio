using System.Collections.Generic;

public class MercenaryHander : Singleton<MercenaryHander>
{
    /// <summary>
    /// key : mercenary Id
    /// value : mercenary data
    /// </summary>
    private Dictionary<int, Mercenary> _dic = new Dictionary<int, Mercenary>();
    private List<Mercenary> _list = new List<Mercenary>();
    public List<Mercenary> list => _list;

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
