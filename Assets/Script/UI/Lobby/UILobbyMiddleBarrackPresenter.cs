using System.Collections.Generic;


public class UILobbyMiddleBarrackPresenter : UILobbyMiddlePresenter<UILobbyMiddleBarrack>
{
    private IReadOnlyList<Mercenary> _mercenaries;

    public UILobbyMiddleBarrackPresenter(UILobbyMiddleBarrack view, UILobbyContext context) : base(view, context)
    {
        _mercenaries = MercenaryManager.instance.list;
    }

    protected override void Bind()
    {
        _view.onClickShow += OnClickShow;
        _view.onItemClick += OnItemClick;
        _view.onGetMercenary = GetMercenaryByIndex;
        _view.onGetItemCount = () => _mercenaries?.Count ?? 0;
    }

    public override void Initialize()
    {
        _context.assetLoader.LoadPrefab("BarrackMercenaryItem", prefab =>
        {
            _view.SetupScroll(prefab);
        });
    }

    protected override void Unbind()
    {
        _view.onClickShow -= OnClickShow;
        _view.onItemClick -= OnItemClick;
        _view.onGetMercenary = null;
        _view.onGetItemCount = null;
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

    private void OnItemClick(Mercenary mercenary)
    {
        _context.popupService.ShowPopup<UIMercenaryDetailPopup>(PopupName.UIMercenaryDetailPopup, mercenary);
    }
}