using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;


public class LocaleService : IDisposable
{
    private readonly Storage _storage;
    private readonly SaveService _saveService;

    public LocaleService(Storage storage, SaveService saveService)
    {
        _storage = storage;
        _saveService = saveService;

        LocalizationSettings.SelectedLocaleChanged += HandleRefreshAll;
    }

    public void Init()
    {
        var code = _storage.data.locale;

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
        LocalizationSettings.SelectedLocaleChanged -= HandleRefreshAll;
    }

    private void HandleRefreshAll(Locale locale)
    {
        _storage.data.locale = locale.Identifier.Code;
        _saveService.RequestSave();
    }
}