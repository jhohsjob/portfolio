using System.Collections.Generic;


public class UILobbyMiddleBarrackPresenter : UILobbyMiddlePresenter<UILobbyMiddleBarrack>
{
    private MercenaryService _mercenaryService;
    private IReadOnlyList<Mercenary> _mercenaries;

    public UILobbyMiddleBarrackPresenter(UILobbyMiddleBarrack view, UILobbyPresenter.Deps deps) : base(view, deps)
    {
        _mercenaryService = deps.mercenaryService;
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
        _deps.assetService.LoadPrefab("BarrackMercenaryScrollItem", prefab =>
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
        _deps.popupService.ShowPopup<UIMercenaryDetailPopup>(
            address: PopupName.UIMercenaryDetailPopup,
            data: mercenary,
            dependencies: new UIMercenaryDetailPopupDependencies
            {
                user = _deps.user,
                currencyService = _deps.currencyService,
                mercenaryService = _mercenaryService,
            });
    }
}