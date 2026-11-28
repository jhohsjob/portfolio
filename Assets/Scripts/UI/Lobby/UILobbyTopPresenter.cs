using System;


public class UILobbyTopPresenter : IDisposable
{
    public sealed class Deps
    {
        public ILocaleService localeService;
        public IDeviceSettings deviceSettings;
        public User user;
        public ICurrencyService currencyService;
        public ISceneService sceneService;
        public IPopupService popupService;
    }


    private readonly UILobbyTop _view;
    private readonly Deps _deps;

    public UILobbyTopPresenter(UILobbyTop view, Deps deps)
    {
        _view = view;
        _deps = deps;

        Bind();
    }

    public void Dispose()
    {
        Unbind();
    }

    private void Bind()
    {
        _view.onClickSetting += OnClickSetting;

        _deps.currencyService.onChanged += OnChangeCurrency;
        _deps.user.OnExpChanged += OnUpdateUserExp;
    }

    private void Unbind()
    {
        _view.onClickSetting -= OnClickSetting;

        _deps.currencyService.onChanged -= OnChangeCurrency;
        _deps.user.OnExpChanged -= OnUpdateUserExp;
    }

    public void Initialize()
    {
        _view.SetLevelText(_deps.user.level);
        var currency = _deps.currencyService;
        _view.SetGoldText(currency.Get(CurrencyType.Gold));
        _view.SetGemText(currency.Get(CurrencyType.Gem));
    }

    private void OnClickSetting()
    {
        _deps.popupService.ShowPopup<UISettingPopup>(
            address: PopupName.UISettingPopup,
            dependencies: new UISettingPopupDependencies
            {
                localeService = _deps.localeService,
                deviceSettings = _deps.deviceSettings,
                sceneService = _deps.sceneService,
            });
    }

    private void OnChangeCurrency(CurrencyType type, int result)
    {
        switch (type)
        {
            case CurrencyType.Gold:
                _view.SetGoldText(result);
                break;
            case CurrencyType.Gem:
                _view.SetGemText(result);
                break;
        }
    }

    private void OnUpdateUserExp(int exp, int level)
    {
        _view.SetLevelText(level);
    }
}