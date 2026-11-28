using System.Collections.Generic;


public class UILobbyMiddleGachaPresenter : UILobbyMiddlePresenter<UILobbyMiddleGacha>
{
    private GachaService _gachaService;
    private IPopupService _popupService;
    private IReadOnlyList<Gacha> _gachas;

    public UILobbyMiddleGachaPresenter(UILobbyMiddleGacha view, UILobbyPresenter.Deps deps) : base(view, deps)
    {
        _gachaService = deps.gachaService;
        _popupService = deps.popupService;
        _gachas = _gachaService.list;
    }

    protected override void Bind()
    {
        _view.onGetGachaData = (index) => _gachas[index];
        _view.onGachaSelected = OnGachaSelected;
        _view.onGetItemCount = () => _gachas?.Count ?? 0;

        _view.onGacha += OnClickGacha;
    }

    protected override void Unbind()
    {
        _view.onGetGachaData = null;
        _view.onGachaSelected = null;
        _view.onGetItemCount = null;

        _view.onGacha -= OnClickGacha;
    }

    public override void Initialize()
    {
        _deps.assetService.LoadPrefab("GachaScrollItem", prefab =>
        {
            _view.SetupGachaScroll(prefab);
        });
    }

    private void OnGachaSelected(int index)
    {
        _view.UpdateMainLayer(_gachas[index].id, _gachas[index].mainUI);
    }

    private async void OnClickGacha(int count)
    {
        int id = _gachas[_view.listIndex].id;
        var resultData = await _deps.gachaService.GachaAsync(id, count);

        if (resultData.result == GachaResult.Success)
        {
            UIGachaResultPopupData data = new UIGachaResultPopupData(id, resultData.rewards);
            _popupService.ShowPopup<UIGachaResultPopup>(
                address: PopupName.UIGachaResultPopup,
                data: data,
                dependencies: new UIGachaResultPopupDependencies
                {
                    mercenaryService = _deps.mercenaryService,
                    gachaService = _gachaService
                });
        }
        else
        {
            _popupService.ShowCommonPopup(LocalizationHelper.GetCommonString("notice"),
                                          LocalizationHelper.GetErrorCodeString(resultData.result.ToString()));
        }
    }
}