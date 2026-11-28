using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;


public interface ILocaleService
{
    event Action<string> changed;

    string currentCode { get; }

    void Apply(string code);
    UniTask<bool> ChangeAsync(string code);
}

public sealed class LocaleService : ILocaleService
{
    public event Action<string> changed;

    private bool _changing;

    public string currentCode => LocalizationSettings.SelectedLocale?.Identifier.Code;

    public LocaleService()
    {
        LocalizationSettings.SelectedLocaleChanged += HandleLocaleChanged;
    }

    public void Apply(string code)
    {
        if (TryFindLocale(code, out var locale) == false)
        {
            return;
        }

        LocalizationSettings.SelectedLocale = locale;
    }

    // 런타임 언어 변경 (설정 팝업 등)
    public async UniTask<bool> ChangeAsync(string code)
    {
        if (_changing == true)
        {
            return false;
        }

        if (TryFindLocale(code, out var locale) == false)
        {
            return false;
        }

        if (LocalizationSettings.SelectedLocale == locale)
        {
            return true;
        }

        _changing = true;
        try
        {
            LocalizationSettings.SelectedLocale = locale;

            await LocalizationSettings.InitializationOperation.ToUniTask();
            await LocalizationSettings.SelectedLocaleAsync.ToUniTask();

            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return false;
        }
        finally
        {
            _changing = false;
        }
    }

    private bool TryFindLocale(string code, out Locale locale)
    {
        locale = null;

        if (string.IsNullOrEmpty(code))
        {
            Debug.LogWarning("LocaleService: empty locale code");
            return false;
        }

        locale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(x => x.Identifier.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

        if (locale == null)
        {
            Debug.LogWarning($"LocaleService: locale not found: {code}");
            return false;
        }

        return true;
    }

    private void HandleLocaleChanged(Locale locale)
    {
        LocalizationHelper.ClearCache();

        changed?.Invoke(locale.Identifier.Code);
    }
}