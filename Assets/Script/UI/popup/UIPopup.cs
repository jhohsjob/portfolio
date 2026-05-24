using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;


public class UIPopup : MonoBehaviour
{
    protected IPopupService _popupService;

    [SerializeField]
    protected Button _btnClose;

    protected bool _isEnableClick;

    public Action onDestroyAction;

    protected virtual void Awake()
    {
        if (_btnClose != null)
        {
            _btnClose.onClick.AddListener(OnClickClose);
        }
    }

    public virtual void OnDestroy()
    {
        onDestroyAction?.Invoke();
    }

    public virtual void Show(object data = null)
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnPopupReady(object data = null)
    {
        _isEnableClick = true;
        DOVirtual.DelayedCall(1f, () =>
        {
            _isEnableClick = false;
        });
    }

    protected virtual void OnClickClose()
    {
        _popupService.ClosePopup(this);
    }

    public void InitDependencies(IPopupService popupService)
    {
        _popupService = popupService;
    }
}