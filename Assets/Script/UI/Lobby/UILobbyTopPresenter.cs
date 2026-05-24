using System;


public class UILobbyTopContext
{
    public IPopupService popupService;
    public ICurrencyService currencyService;
}

public class UILobbyTopPresenter : IDisposable
{
    private readonly UILobbyTop _view;
    private readonly UILobbyTopContext _context;

    public UILobbyTopPresenter(UILobbyTop view, UILobbyTopContext context)
    {
        _view = view;
        _context = context;

        Bind();
    }

    private void Bind()
    {
        _view.onClickSetting += OnClickSetting;

        _context.currencyService.onChanged += OnChangeCurrency;
    }

    public void Initialize()
    {
        int currentGold = _context.currencyService.Get(CurrencyType.Gold);
        _view.goldView.SetGoldText(currentGold);
    }

    public void Dispose()
    {
        _view.onClickSetting -= OnClickSetting;

        _context.currencyService.onChanged -= OnChangeCurrency;
    }

    private void OnChangeCurrency(CurrencyType type, int result)
    {
        if (type == CurrencyType.Gold)
        {
            _view.goldView.SetGoldText(result);
        }
    }

    private void OnClickSetting()
    {
        _context.popupService.ShowPopup<UISettingPopup>(PopupName.UISettingPopup);
    }
}