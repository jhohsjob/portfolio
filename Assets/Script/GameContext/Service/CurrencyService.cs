using System;
using System.Collections.Generic;


public class CurrencyService : ICurrencyService
{
    public User _user;
    public SaveService _saveService;

    private Dictionary<CurrencyType, int> _currencies = new();

    public event Action<CurrencyType, int> onChanged;

    public CurrencyService(User user, SaveService saveService)
    {
        _user = user;
        _saveService = saveService;
    }

    public void Init()
    {
        _currencies.Clear();

        _currencies[CurrencyType.Gold] = _user.gold;
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
        switch (type)
        {
            case CurrencyType.Gold:
                _user.SetGold(value);
                break;
        }

        _saveService.RequestSave();
    }
}