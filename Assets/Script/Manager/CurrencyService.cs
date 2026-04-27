using System;
using System.Collections.Generic;


public class CurrencyService
{
    private Dictionary<CurrencyType, int> _currencies = new();

    public event Action<CurrencyType, int> onChanged;

    public void Init()
    {
        var data = Client.storage.data.player;

        _currencies.Clear();

        _currencies[CurrencyType.Gold] = data.gold;
    }

    public int Get(CurrencyType type)
    {
        return _currencies.TryGetValue(type, out var value) ? value : 0;
    }

    public bool HasEnough(CurrencyType type, int amount)
    {
        return Get(type) >= amount;
    }

    public bool Change(CurrencyType type, int amount, bool save = true)
    {
        int current = Get(type);
        int next = current + amount;

        if (next < 0)
        {
            return false;
        }

        _currencies[type] = next;

        if (save)
        {
            Save(type, next);
        }

        onChanged?.Invoke(type, next);

        return true;
    }

    private void Save(CurrencyType type, int value)
    {
        var data = Client.storage.data.player;

        switch (type)
        {
            case CurrencyType.Gold:
                data.gold = value;
                break;
        }

        StorageSaveManager.RequestSave();
    }
}