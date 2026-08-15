using System;
using UnityEngine;


public class UIMercenaryDetailPopupPresenter : IDisposable
{
    private readonly UIMercenaryDetailPopup _view;

    private Mercenary _currentMercenary;

    private const float _dragThreshold = 150f;

    public UIMercenaryDetailPopupPresenter(UIMercenaryDetailPopup view)
    {
        _view = view;

        Bind();
    }

    public void OnPopupReady(object data, IAssetLoader assetLoader)
    {
        if (data is not Mercenary mercenary)
        {
            return;
        }

        _currentMercenary = mercenary;

        assetLoader.LoadPrefab("UIMercenaryDetailPopupSkillScrollItem", prefab =>
        {
            _view.SetupSkillScroll(prefab, 0);
        });

        Refresh();
    }

    private void Bind()
    {
        _view.onClickLeft += OnClickLeft;
        _view.onClickRight += OnClickRight;
        _view.onDrag += OnDrag;

        _view.onClickSkillItem += OnClickSkillItem;

        _view.onGetSkillCount = () => _currentMercenary.skillDataList.Count;
        _view.onGetSkillData = (index) => _currentMercenary.skillDataList[index];  
    }

    private void Unbind()
    {
        _view.onClickLeft -= OnClickLeft;
        _view.onClickRight -= OnClickRight;
        _view.onDrag -= OnDrag;

        _view.onClickSkillItem -= OnClickSkillItem;

        _view.onGetSkillCount = null;
        _view.onGetSkillData = null;
    }

    public void Dispose()
    {
        Unbind();
    }

    private void Refresh()
    {
        _view.ShowMercenary(_currentMercenary);
    }

    private void OnClickLeft()
    {
        _currentMercenary = MercenaryManager.instance.GetPrev(_currentMercenary);
        Refresh();
    }

    private void OnClickRight()
    {
        _currentMercenary = MercenaryManager.instance.GetNext(_currentMercenary);
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