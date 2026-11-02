using System;


public class UILobbyTopContext
{
    public User user;
    public ICurrencyService currencyService;
    public IPopupService popupService;
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

    public void Dispose()
    {
        Unbind();
    }

    private void Bind()
    {
        _view.onClickSetting += OnClickSetting;

        _context.currencyService.onChanged += OnChangeCurrency;
        _context.user.OnExpChanged += OnUpdateUserExp;
    }

    private void Unbind()
    {
        _view.onClickSetting -= OnClickSetting;

        _context.currencyService.onChanged -= OnChangeCurrency;
        _context.user.OnExpChanged -= OnUpdateUserExp;
    }

    public void Initialize()
    {
        _view.SetLevelText(_context.user.level);
        var currency = _context.currencyService;
        _view.SetGoldText(currency.Get(CurrencyType.Gold));
        _view.SetGemText(currency.Get(CurrencyType.Gem));
    }

    private void OnClickSetting()
    {
        _context.popupService.ShowPopup<UISettingPopup>(PopupName.UISettingPopup);
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