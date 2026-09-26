using System;


public class UILobbyTopContext
{
    public User user;
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

    public void Dispose()
    {
        Unbind();
    }

    private void Bind()
    {
        _view.onClickSetting += OnClickSetting;

        _context.currencyService.onChanged += OnChangeCurrency;

        EventHelper.AddEventListener(EventName.UpdateUserPvo, OnUpdateUserPvo);
    }

    private void Unbind()
    {
        _view.onClickSetting -= OnClickSetting;

        _context.currencyService.onChanged -= OnChangeCurrency;

        EventHelper.RemoveEventListener(EventName.UpdateUserPvo, OnUpdateUserPvo);
    }

    public void Initialize()
    {
        _view.SetLevelText(_context.user.level);
        int currentGold = _context.currencyService.Get(CurrencyType.Gold);
        _view.SetGoldText(currentGold);
    }

    private void OnClickSetting()
    {
        _context.popupService.ShowPopup<UISettingPopup>(PopupName.UISettingPopup);
    }

    private void OnChangeCurrency(CurrencyType type, int result)
    {
        if (type == CurrencyType.Gold)
        {
            _view.SetGoldText(result);
        }
    }

    private void OnUpdateUserPvo(object sender, object data)
    {
        if (sender is not User user)
        {
            return;
        }
        
        _view.SetLevelText(user.level);
        _view.SetGoldText(user.gold);
    }
}