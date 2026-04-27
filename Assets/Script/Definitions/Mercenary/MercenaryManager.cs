using System.Collections.Generic;
using System.Threading.Tasks;


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

    public void ApplySaveData(Dictionary<int, MercenarySaveData> saveDatas)
    {
        foreach (var mercenary in _dic.Values)
        {
            if (saveDatas.TryGetValue(mercenary.id, out var saveData))
            {
                mercenary.ApplySaveData(saveData);
            }
        }
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
        _dic.TryGetValue(id, out var mercenary);
        return mercenary;
    }

    public bool Has(int id)
    {
        if (_dic.TryGetValue(id, out var mercenary))
        {
            return mercenary.isOwned;
        }
        return false;
    }

    public async Task<bool> Acquire(int id)
    {
        if (_dic.TryGetValue(id, out var mercenary))
        {
            if (mercenary.Acquire() == false)
            {
                return false;
            }

            var runtimeData = mercenary.GetSaveData();

            await Client.mercenaryStorage.Update(runtimeData);

            return true;
        }

        return false;
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
