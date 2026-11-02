using System;
using System.Collections.Generic;


public class UIGachaResultPopupPresenter : IDisposable
{
    private readonly UIGachaResultPopup _view;

    private IAssetService _assetService;
    private GachaService _gachaService;
    private IPopupService _popupService;

    private int _gachaId;
    private IReadOnlyList<RewardBase> _rewards;

    public UIGachaResultPopupPresenter(UIGachaResultPopup view)
    {
        _view = view;

        Bind();
    }

    public void Dispose()
    {
        Unbind();
    }

    public void InitDependencies(UIGachaResultPopupDependencies gachaDependencies)
    {
        _assetService = gachaDependencies.assetService;
        _gachaService = gachaDependencies.gachaService;
        _popupService = gachaDependencies.popupService;
    }

    public void OnPopupReady(UIGachaResultPopupData popupData)
    {
        _gachaId = popupData.gachaId;
        _rewards = popupData.rewards;

        _assetService.LoadPrefab("UIGachaResultPopupRewardItem", prefab =>
        {
            _view.SetupScroll(prefab);
        });
    }

    private void Bind()
    {
        _view.onClickGacha += OnClickGacha;
        _view.onGetReward = GetRewardByIndex;
        _view.onGetItemCount = () => _rewards?.Count ?? 0;
    }

    private void Unbind()
    {
        _view.onClickGacha -= OnClickGacha;
        _view.onGetReward = null;
        _view.onGetItemCount = null;
    }

    private RewardBase GetRewardByIndex(int index)
    {
        if (_rewards == null || index < 0 || index >= _rewards.Count)
        {
            return null;
        }

        return _rewards[index];
    }

    private async void OnClickGacha(int count)
    {
        var resultData = await _gachaService.GachaAsync(_gachaId, count);
        if (resultData.result == GachaResult.Success)
        {
            _rewards = resultData.rewards;
        }
        else
        {
            _popupService.ShowCommonPopup(LocalManager.GetCommonString("notice"),
                                          LocalManager.GetErrorCodeString(resultData.result.ToString()));
        }

        _view.OnGachaCallback();
    }
}