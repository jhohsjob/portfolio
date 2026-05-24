using System;

public interface ICurrencyService
{
    event Action<CurrencyType, int> onChanged;

    int Get(CurrencyType type);
    bool HasEnough(CurrencyType type, int amount);

    bool Change(CurrencyType type, int amount, bool save = true);
}