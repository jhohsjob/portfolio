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
        _view.onGetMercenary = GetMercenaryByIndex;
        _view.onGetItemCount = () => _mercenaries?.Count ?? 0;
    }

    protected override void Unbind()
    {
        _view.onClickShow -= OnClickShow;
        _view.onClickItem -= OnClickItem;
        _view.onGetMercenary = null;
        _view.onGetItemCount = null;
    }

    public override void Initialize()
    {
        _context.assetService.LoadPrefab("BarrackMercenaryScrollItem", prefab =>
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
        _context.popupService.ShowPopup<UIMercenaryDetailPopup>(
            address: PopupName.UIMercenaryDetailPopup,
            data: mercenary,
            dependencies: new UIMercenaryDetailPopupDependencies
            {
                user = _context.user,
                currencyService = _context.currencyService,
                mercenaryService = _mercenaryService,
            });
    }
}