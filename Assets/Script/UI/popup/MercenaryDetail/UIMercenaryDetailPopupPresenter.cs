using System;
using UnityEngine;
using UnityEngine.Localization.Settings;


public class UIMercenaryDetailPopupPresenter : IDisposable
{
    private readonly UIMercenaryDetailPopup _view;

    private IAssetLoader _assetLoader;
    private IPopupService _popupService;
    private MercenaryService _mercenaryService;
    private User _user;

    private Mercenary _currentMercenary;
    public int currentLevel => _currentMercenary.level;

    private const float _dragThreshold = 150f;

    public UIMercenaryDetailPopupPresenter(UIMercenaryDetailPopup view)
    {
        _view = view;

        Bind();
    }

    public void Dispose()
    {
        Unbind();
    }

    public void InitDependencies(IAssetLoader assetLoader, IPopupService popupService, MercenaryService mercenaryService, User user)
    {
        _assetLoader = assetLoader;
        _popupService = popupService;
        _mercenaryService = mercenaryService;
        _user = user;
    }

    public void OnPopupReady(object data)
    {
        if (data is not Mercenary mercenary)
        {
            return;
        }

        _currentMercenary = mercenary;

        _assetLoader.LoadPrefab("UIMercenaryDetailPopupSkillScrollItem", prefab =>
        {
            _view.SetupSkillScroll(prefab, 0);
        });

        Refresh();
    }

    private void Bind()
    {
        _view.onClickLevelUp += OnClickLevelUp;
        _view.onClickLevelUpMax += OnClickLevelUpMax;
        _view.onClickLevelReset += OnClickLevelReset;

        _view.onClickLeft += OnClickLeft;
        _view.onClickRight += OnClickRight;
        _view.onDrag += OnDrag;

        _view.onLongPressLevelUp += OnClickLevelUp;

        _view.onClickSkillItem += OnClickSkillItem;

        _view.onGetSkillCount = () => _currentMercenary.skillDataList.Count;
        _view.onGetSkillData = (index) => _currentMercenary.skillDataList[index];  
    }

    private void Unbind()
    {
        _view.onClickLevelUp -= OnClickLevelUp;
        _view.onClickLevelUpMax -= OnClickLevelUpMax;
        _view.onClickLevelReset -= OnClickLevelReset;

        _view.onClickLeft -= OnClickLeft;
        _view.onClickRight -= OnClickRight;
        _view.onDrag -= OnDrag;

        _view.onLongPressLevelUp -= OnClickLevelUp;

        _view.onClickSkillItem -= OnClickSkillItem;

        _view.onGetSkillCount = null;
        _view.onGetSkillData = null;
    }

    private void Refresh()
    {
        int availableMaxLevel = _mercenaryService.GetAvailableMaxLevel(_currentMercenary.level, _user.level, _user.gold);

        _view.ShowMercenary(_currentMercenary, availableMaxLevel);
    }

    public bool CanLevelUp()
    {
        int availableMaxLevel = _mercenaryService.GetAvailableMaxLevel(_currentMercenary.level, _user.level, _user.gold);

        return _currentMercenary.isOwned && _currentMercenary.level < availableMaxLevel;
    }

    public bool CanPreviewLevelUp(int previewLevel)
    {
        int availableMaxLevel = _mercenaryService.GetAvailableMaxLevel(_currentMercenary.level, _user.level, _user.gold);

        return _currentMercenary.isOwned && previewLevel < availableMaxLevel;
    }

    public bool CanLevelReset()
    {
        return _currentMercenary.isOwned && _currentMercenary.level > 1;
    }

    private void LevelUp(MercenaryLevelUpResult result)
    {
        switch (result)
        {
            case MercenaryLevelUpResult.Success:
                Refresh();
                break;

            default:
                _popupService.ShowCommonPopup("알림", LocalizationSettings.StringDatabase.GetLocalizedString("ErrorCodeTable", result.ToString()));
                break;
        }
    }

    private async void OnClickLevelUp()
    {
        var result = await _mercenaryService.LevelUp(_currentMercenary.id, _currentMercenary.level + 1);

        LevelUp(result);
    }

    private async void OnClickLevelUp(int targetLevel)
    {
        var result = await _mercenaryService.LevelUp(_currentMercenary.id, targetLevel);

        LevelUp(result);
    }

    private async void OnClickLevelUpMax()
    {
        int availableMaxLevel = _mercenaryService.GetAvailableMaxLevel(_currentMercenary.level, _user.level, _user.gold);

        var result = await _mercenaryService.LevelUp(_currentMercenary.id, availableMaxLevel);

        LevelUp(result);
    }

    private async void OnClickLevelReset()
    {
        var result = await _mercenaryService.LevelReset(_currentMercenary.id);

        switch (result)
        {
            case MercenaryLevelResetResult.Success:
                Refresh();
                break;

            default:
                _popupService.ShowCommonPopup("알림", LocalizationSettings.StringDatabase.GetLocalizedString("ErrorCodeTable", result.ToString()));
                break;
        }
    }

    private void OnClickLeft()
    {
        _currentMercenary = _mercenaryService.GetPrev(_currentMercenary);
        Refresh();
    }

    private void OnClickRight()
    {
        _currentMercenary = _mercenaryService.GetNext(_currentMercenary);
        Refresh();
    }

    private void OnDrag(float deltaX)
    {
        if (Mathf.Abs(deltaX) < _dragThreshold)
        {
            return;
        }

        if (deltaX > 0)
        {
            OnClickRight();
        }
        else if (deltaX < 0)
        {
            OnClickLeft();
        }
    }

    private void OnClickSkillItem(SkillData skillData, Transform itemTransform)
    {
        _view.ShowSkillInfo(skillData, itemTransform);
    }
}