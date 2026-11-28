using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    /// <summary>
    /// 서버 요청 공통 흐름: busy 가드 → 요청 → (해제 여부 확인) → 실패 시 에러 팝업 → Refresh.
    /// refreshOnFailure가 true면 실패해도 화면을 갱신한다. (레벨업은 실패 시에도 최대 레벨 표기 등을 갱신)
    /// </summary>
    private async void RunBusy<TResult>(Func<Task<TResult>> request, TResult success, bool refreshOnFailure = false) where TResult : struct, Enum
    {
        if (TryBeginBusy() == false)
        {
            return;
        }

        try
        {
            var result = await request();
            if (_disposed == true)
            {
                return;
            }

            bool isSuccess = EqualityComparer<TResult>.Default.Equals(result, success);
            if (isSuccess == false)
            {
                ShowError(result.ToString());
            }

            if (isSuccess == true || refreshOnFailure == true)
            {
                Refresh();
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

    private void OnClickLevelUp()
    {
        OnClickLevelUp(_currentMercenary.level + 1);
    }

    private void OnClickLevelUp(int targetLevel)
    {
        RunBusy(() => _mercenaryService.LevelUp(_currentMercenary.Id, targetLevel),
                MercenaryLevelUpResult.Success,
                refreshOnFailure: true);
    }

    private void OnClickLevelUpMax()
    {
        RunBusy(() => _mercenaryService.LevelUp(_currentMercenary.Id, GetAvailableMaxLevel()),
                MercenaryLevelUpResult.Success,
                refreshOnFailure: true);
    }

    private void OnClickLevelReset()
    {
        RunBusy(() => _mercenaryService.LevelReset(_currentMercenary.Id),
                MercenaryLevelResetResult.Success);
    }

    private void OnClickGradeUp()
    {
        RunBusy(() => _mercenaryService.GradeUp(_currentMercenary.Id),
                MercenaryGradeUpResult.Success);
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
