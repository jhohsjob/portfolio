using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;


public class UISettingPopup : UIPopup
{
    [SerializeField]
    protected ToggleGroup _tgLocale;

    private LocaleToggle[] _toggles;

    protected override void Awake()
    {
        base.Awake();

        _toggles = _tgLocale.GetComponentsInChildren<LocaleToggle>();
    }

    public override void OnPopupReady(object data = null)
    {
        base.OnPopupReady(data);

        foreach (var t in _toggles)
        {
            t.toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    ChangeLocale(t.localeCode);
                }
            });
        }

        InitCurrentLocale();
    }

    private void InitCurrentLocale()
    {
        var current = LocalizationSettings.SelectedLocale;

        foreach (var t in _toggles)
        {
            var locale = LocalizationSettings.AvailableLocales.GetLocale(t.localeCode);

            if (locale == current)
            {
                t.toggle.isOn = true;
                break;
            }
        }
    }

    private void ChangeLocale(string code)
    {
        var locale = LocalizationSettings.AvailableLocales.GetLocale(code);

        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
        }
    }
}