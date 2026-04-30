using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;


public class LocaleManager : IDisposable
{
    public LocaleManager()
    {
        LocalizationSettings.SelectedLocaleChanged += RefreshAll;
    }

    public void Init()
    {
        var code = Client.storage.data.locale;

        var locale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(x => x.Identifier.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
        }
        else
        {
            Debug.LogWarning($"Locale not found: {code}");
        }
    }

    public void Dispose()
    {
        LocalizationSettings.SelectedLocaleChanged -= RefreshAll;
    }

    private void RefreshAll(Locale locale)
    {
        EventHelper.Send(EventName.LocaleChanged, this);
    }
}