using System.Collections.Generic;


public class UILobbyMiddleBarrackPresenter : UILobbyMiddlePresenter<UILobbyMiddleBarrack>
{
    private MercenaryService _mercenaryService;
    private IReadOnlyList<Mercenary> _mercenaries;

    public UILobbyMiddleBarrackPresenter(UILobbyMiddleBarrack view, UILobbyContext context) : base(view, context)
    {
        _mercenaryService = context.mercenaryService;
        _mercenaries = _mercenaryService.list;
    }

    protected override void Bind()
    {
        _view.onClickShow += OnClickShow;
        _view.onClickItem += OnClickItem;
        _view.onClickBuy += OnClickBuy;
        _view.onGetMercenary = GetMercenaryByIndex;
        _view.onGetItemCount = () => _mercenaries?.Count ?? 0;
    }

    protected override void Unbind()
    {
        _view.onClickShow -= OnClickShow;
        _view.onClickItem -= OnClickItem;
        _view.onClickBuy -= OnClickBuy;
        _view.onGetMercenary = null;
        _view.onGetItemCount = null;
    }

    public override void Initialize()
    {
        _context.assetLoader.LoadPrefab("BarrackMercenaryItem", prefab =>
        {
            _view.SetupScroll(prefab);
        });
    }

    private Mercenary GetMercenaryByIndex(int index)
    {
        if (_mercenaries == null || index < 0 || index >= _mercenaries.Count)
        {
            return null;
        }

        return _mercenaries[index];
    }

    private void OnClickShow()
    {
        _view.RefreshScroll();
    }

    private void OnClickItem(Mercenary mercenary)
    {
        _context.popupService.ShowPopup<UIMercenaryDetailPopup>(PopupName.UIMercenaryDetailPopup, mercenary, popup =>
        {
            popup.AddDependencies(_context.mercenaryService, _context.user);
        });
    }

    private async void OnClickBuy(Mercenary mercenary)
    {
        var result = await _mercenaryService.Acquire(mercenary.id);

        if (result == MercenaryAcquireResult.Success)
        {
            _view.RefreshScroll();
        }
        else
        {
            // todo : error
        }
    }
}