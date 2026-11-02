using System;
using System.Collections.Generic;


public class CurrencyService : ICurrencyService
{
    private User _user;

    private Dictionary<CurrencyType, int> _currencies = new();

    public event Action<CurrencyType, int> onChanged;

    public CurrencyService(User user)
    {
        _user = user;

        _user.onGoldChanged += OnGoldChanged;
        _user.onGemChanged += OnGemChanged;
    }

    public int Get(CurrencyType type)
    {
        return _currencies.TryGetValue(type, out var value) ? value : 0;
    }

    public bool HasEnough(CurrencyType type, int amount)
    {
        return Get(type) >= amount;
    }

    private void UpdateCurrency(CurrencyType type, int value)
    {
        _currencies[type] = value;
        onChanged?.Invoke(type, value);
    }

    private void OnGoldChanged(int gold)
    {
        UpdateCurrency(CurrencyType.Gold, gold);
    }

    private void OnGemChanged(int gem)
    {
        UpdateCurrency(CurrencyType.Gem, gem);
    }
}