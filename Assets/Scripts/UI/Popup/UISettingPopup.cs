using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;


public class UISettingPopupDependencies : UIPopupDependencies
{
    public ILocaleService localeService;
    public IDeviceSettings deviceSettings;
    public ISceneService sceneService;
}

public class UISettingPopup : UIPopup
{
    [Header("Locale")]
    [SerializeField]
    protected ToggleGroup _tgLocale;

    [Header("Sound")]
    [SerializeField]
    private Slider _sliderBgm;
    [SerializeField]
    private Slider _sliderSfx;

    [Header("Account")]
    [SerializeField]
    private Button _btnLogout;

    private LocaleToggle[] _toggles;

    private ILocaleService _localeService;
    private IDeviceSettings _deviceSettings;
    private ISceneService _sceneService;

    protected override void Awake()
    {
        base.Awake();

        _toggles = _tgLocale.GetComponentsInChildren<LocaleToggle>();
    }

    public override void InitDependencies(UIPopupDependencies dependencies)
    {
        base.InitDependencies(dependencies);

        if (dependencies is UISettingPopupDependencies settingDependencies)
        {
            _localeService = settingDependencies.localeService;
            _deviceSettings = settingDependencies.deviceSettings;
            _sceneService = settingDependencies.sceneService;
        }
    }

    public override void OnPopupReady(object data = null)
    {
        base.OnPopupReady(data);

        InitCurrentLocale();
        InitSound();

        BindLocale();
        BindSound();
        BindLogout();
    }

    #region Locale

    private void InitCurrentLocale()
    {
        if (_localeService == null)
        {
            foreach (var t in _toggles) t.toggle.interactable = false;
            return;
        }

        string current = _localeService.currentCode;

        foreach (var t in _toggles)
        {
            if (string.Equals(t.localeCode, current, StringComparison.OrdinalIgnoreCase))
            {
                t.toggle.SetIsOnWithoutNotify(true);
                break;
            }
        }
    }

    private void BindLocale()
    {
        if (_localeService == null)
        {
            return;
        }

        foreach (var t in _toggles)
        {
            var code = t.localeCode;
            t.toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    ChangeLocale(code);
                }
            });
        }
    }

    private async void ChangeLocale(string code)
    {
        SetLocaleTogglesInteractable(false);

        bool success = await _localeService.ChangeAsync(code);
        if (this == null)
        {
            return; // 변경 중 팝업이 닫힘
        }

        if (success == false)
        {
            InitCurrentLocale();
        }

        SetLocaleTogglesInteractable(true);
    }
    
    private void SetLocaleTogglesInteractable(bool value)
    {
        foreach (var t in _toggles)
        {
            t.toggle.interactable = value;
        }
    }

    #endregion

    #region Sound

    private void InitSound()
    {
        bool ready = _deviceSettings != null;

        _sliderBgm.interactable = ready;
        _sliderSfx.interactable = ready;

        if (ready == false)
        {
            return;
        }

        _sliderBgm.SetValueWithoutNotify(_deviceSettings.bgmVolume);
        _sliderSfx.SetValueWithoutNotify(_deviceSettings.sfxVolume);
    }

    private void BindSound()
    {
        if (_deviceSettings == null)
        {
            return;
        }

        _sliderBgm.onValueChanged.AddListener(_deviceSettings.SetBgmVolume);
        _sliderSfx.onValueChanged.AddListener(_deviceSettings.SetSfxVolume);
    }

    #endregion

    #region Logout

    private void BindLogout()
    {
        _btnLogout.interactable = _sceneService != null;
        _btnLogout.onClick.AddListener(OnClickLogout);
    }

    private void OnClickLogout()
    {
        // todo : login/logout
        return;
        _btnLogout.interactable = false;

        _sceneService.LoadBootScene();
    }

    #endregion
}