using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;


public sealed class LocaleService : IDisposable
{
    public event Action<string> changed;

    private bool _subscribed;

    public void Apply(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            Debug.LogWarning("LocaleService.Apply called with empty locale code");
            return;
        }

        var locale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(x => x.Identifier.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
        }
        else
        {
            Debug.LogWarning($"Locale not found: {code}");
        }

        if (_subscribed == false)
        {
            LocalizationSettings.SelectedLocaleChanged += HandleLocaleChanged;
            _subscribed = true;
        }
    }

    public void Dispose()
    {
        if (_subscribed == false)
        {
            return;
        }

        LocalizationSettings.SelectedLocaleChanged -= HandleLocaleChanged;
        _subscribed = false;
    }

    private void HandleLocaleChanged(Locale locale)
    {
        LocalizationHelper.ClearCache();

        changed?.Invoke(locale.Identifier.Code);
    }
}