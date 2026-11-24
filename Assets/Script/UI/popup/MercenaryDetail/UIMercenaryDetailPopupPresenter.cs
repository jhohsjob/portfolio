using System;
using System.Collections.Generic;
using UnityEngine;


public class UIMercenaryDetailPopupPresenter : IDisposable
{
    private readonly UIMercenaryDetailPopup _view;
    private readonly Transform _pivot;

    private IAssetService _assetService;
    private User _user;
    private ICurrencyService _currencyService;
    private MercenaryService _mercenaryService;
    private IPopupService _popupService;

    private readonly Dictionary<int, MercenaryViewPool> _pools = new();

    private Mercenary _currentMercenary;
    private MercenaryView _currentView;
    private int _currentViewId = -1;

    private bool _isBusy;
    private bool _disposed;

    public int currentLevel => _currentMercenary == null ? 0 : _currentMercenary.level;

    public UIMercenaryDetailPopupPresenter(UIMercenaryDetailPopup view, Transform pivot)
    {
        _view = view;
        _pivot = pivot;

        Bind();
    }

    public void Dispose()
    {
        _disposed = true;

        Unbind();
        ReleaseCurrentView();

        foreach (var pool in _pools.Values)
        {
            pool.Dispose();
        }
        _pools.Clear();
    }

    public void InitDependencies(UIMercenaryDetailPopupDependencies mercenaryDependencies)
    {
        _assetService = mercenaryDependencies.assetService;
        _user = mercenaryDependencies.user;
        _currencyService = mercenaryDependencies.currencyService;
        _mercenaryService = mercenaryDependencies.mercenaryService;
        _popupService = mercenaryDependencies.popupService;
    }

    public void OnPopupReady(object data)
    {
        if (data is not Mercenary mercenary)
        {
            return;
        }

        _currentMercenary = mercenary;

        _assetService.LoadPrefab("UIMercenaryDetailPopupSkillScrollItem", prefab =>
        {
            if (_disposed == true)
            {
                return;
            }

            _view.SetupSkillScroll(prefab, 0);
        });
    }

    public void Show()
    {
        Refresh();
    }

    #region Bind

    private void Bind()
    {
        _view.onClickLevelUp += OnClickLevelUp;
        _view.onClickLevelUpMax += OnClickLevelUpMax;
        _view.onClickLevelReset += OnClickLevelReset;
        _view.onClickGradeUp += OnClickGradeUp;

        _view.onClickLeft += OnClickLeft;
        _view.onClickRight += OnClickRight;
        _view.onDrag += OnDrag;

        _view.onLongPressLevelUp += OnClickLevelUp;

        _view.onClickSkillItem += OnClickSkillItem;

        _view.onGetSkillCount = () => _currentMercenary?.skillDataList.Count ?? 0;
        _view.onGetSkillData = index => _currentMercenary?.skillDataList[index];
    }

    private void Unbind()
    {
        _view.onClickLevelUp -= OnClickLevelUp;
        _view.onClickLevelUpMax -= OnClickLevelUpMax;
        _view.onClickLevelReset -= OnClickLevelReset;
        _view.onClickGradeUp -= OnClickGradeUp;

        _view.onClickLeft -= OnClickLeft;
        _view.onClickRight -= OnClickRight;
        _view.onDrag -= OnDrag;

        _view.onLongPressLevelUp -= OnClickLevelUp;

        _view.onClickSkillItem -= OnClickSkillItem;

        _view.onGetSkillCount = null;
        _view.onGetSkillData = null;
    }

    #endregion

    #region View / Pool

    private MercenaryViewPool GetPool(int id, GameObject original)
    {
        if (_pools.TryGetValue(id, out var pool))
        {
            return pool;
        }

        pool = new MercenaryViewPool(original, _pivot);
        _pools.Add(id, pool);

        return pool;
    }

    private void ReleaseCurrentView()
    {
        if (_currentView == null)
        {
            _currentViewId = -1;
            return;
        }

        if (_pools.TryGetValue(_currentViewId, out var pool))
        {
            pool.Release(_currentView);
        }

        _currentView = null;
        _currentViewId = -1;
    }

    private int GetAvailableMaxLevel()
    {
        return _mercenaryService.GetAvailableMaxLevel(_currentMercenary.level, _user.level, _currencyService.Get(CurrencyType.Gold));
    }

    private void Refresh()
    {
        Debug.Log("Refresh");

        if (_disposed || _currentMercenary == null)
        {
            return;
        }

        if (_currentView == null || _currentViewId != _currentMercenary.Id)
        {
            ReleaseCurrentView();

            _currentView = GetPool(_currentMercenary.Id, _currentMercenary.original).Get();
            _currentViewId = _currentMercenary.Id;
        }

        _view.ShowMercenary(_currentMercenary, _currentView, BuildViewState());
    }

    private MercenaryViewState BuildViewState()
    {
        int availableMaxLevel = GetAvailableMaxLevel();
        bool isOwned = _currentMercenary.isOwned;
        bool isGradeMax = _currentMercenary.grade >= GameConfig.MercenaryGradeLimit;
        var progress = isGradeMax ? (current: 0, required: 0) : _mercenaryService.GetGradeUpProgress(_currentMercenary.grade, _currentMercenary.shardCount);

        return new MercenaryViewState
        {
            availableMaxLevel = availableMaxLevel,
            canLevelUp = isOwned && _currentMercenary.level < availableMaxLevel,
            canLevelReset = isOwned && _currentMercenary.level > 1,
            canGradeUp = isOwned && !isGradeMax && _mercenaryService.CanGradeUp(_currentMercenary.grade, _currentMercenary.grade + 1, _currentMercenary.shardCount),
            gradeUpProgress = progress.current,
            gradeUpRequired = progress.required,
        };
    }

    #endregion

    public bool CanPreviewLevelUp(int previewLevel)
    {
        if (_currentMercenary == null)
        {
            return false;
        }

        return _currentMercenary.isOwned && previewLevel < GetAvailableMaxLevel();
    }

    #region Requests

    private bool TryBeginBusy()
    {
        if (_isBusy == true || _disposed == true)
        {
            return false;
        }

        _isBusy = true;
        return true;
    }

    private void ShowError(string resultName)
    {
        _popupService.ShowCommonPopup(LocalizationHelper.GetCommonString("notice"), LocalizationHelper.GetErrorCodeString(resultName));
    }

    private void OnClickLevelUp()
    {
        OnClickLevelUp(_currentMercenary.level + 1);
    }

    private async void OnClickLevelUp(int targetLevel)
    {
        if (TryBeginBusy() == false)
        {
            return;
        }

        try
        {
            var result = await _mercenaryService.LevelUp(_currentMercenary.Id, targetLevel);
            if (_disposed == true)
            {
                return;
            }

            if (result != MercenaryLevelUpResult.Success)
            {
                ShowError(result.ToString());
            }

            Refresh();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            _isBusy = false;
        }
    }

    private async void OnClickLevelUpMax()
    {
        if (TryBeginBusy() == false)
        {
            return;
        }

        try
        {
            var result = await _mercenaryService.LevelUp(_currentMercenary.Id, GetAvailableMaxLevel());
            if (_disposed == true)
            {
                return;
            }

            if (result != MercenaryLevelUpResult.Success)
            {
                ShowError(result.ToString());
            }

            Refresh();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            _isBusy = false;
        }
    }

    private async void OnClickLevelReset()
    {
        if (TryBeginBusy() == false)
        {
            return;
        }

        try
        {
            var result = await _mercenaryService.LevelReset(_currentMercenary.Id);
            if (_disposed == true)
            {
                return;
            }

            if (result == MercenaryLevelResetResult.Success)
            {
                Refresh();
            }
            else
            {
                ShowError(result.ToString());
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            _isBusy = false;
        }
    }

    private async void OnClickGradeUp()
    {
        if (TryBeginBusy() == false)
        {
            return;
        }

        try
        {
            var result = await _mercenaryService.GradeUp(_currentMercenary.Id);
            if (_disposed == true)
            {
                return;
            }

            if (result == MercenaryGradeUpResult.Success)
            {
                Refresh();
            }
            else
            {
                ShowError(result.ToString());
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            _isBusy = false;
        }
    }

    #endregion

    #region Navigation

    private void OnClickLeft()
    {
        Move(_mercenaryService.GetPrev(_currentMercenary));
    }

    private void OnClickRight()
    {
        Move(_mercenaryService.GetNext(_currentMercenary));
    }

    private void Move(Mercenary target)
    {
        if (_isBusy == true || _disposed == true || target == null)
        {
            return;
        }

        _currentMercenary = target;
        Refresh();
    }

    private void OnDrag(float deltaX)
    {
        float threshold = Screen.width * 0.15f;

        if (Mathf.Abs(deltaX) < threshold)
        {
            return;
        }

        if (deltaX > 0)
        {
            OnClickRight();
        }
        else
        {
            OnClickLeft();
        }
    }

    private void OnClickSkillItem(SkillData skillData, Transform itemTransform)
    {
        _view.ShowSkillInfo(skillData, itemTransform);
    }

    #endregion
}
