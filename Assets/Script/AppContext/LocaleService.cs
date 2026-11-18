using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;


// 앱 수명. LocalizationSettings에 언어를 적용하고, 변경을 이벤트로 알린다.
// 저장이나 설정 저장소는 모른다. (SoundService와 같은 "적용기")
public sealed class LocaleService : IDisposable
{
    public event Action<string> changed;

    private bool _subscribed;

    // 저장된 언어를 적용한다. Localization 초기화 이후에 호출해야 한다.
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

        // 초기 적용이 끝난 뒤에 구독해서, 적용 자체가 Changed를 일으키지 않게 한다.
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
        changed?.Invoke(locale.Identifier.Code);
    }
}