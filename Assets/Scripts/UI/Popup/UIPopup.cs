using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;


public class UIPopupDependencies
{
    public IAssetService assetService;
    public ISoundService soundService;
    public IPopupService popupService;
}

public class UIPopup : MonoBehaviour
{
    protected IAssetService _assetService;
    protected ISoundService _soundService;
    protected IPopupService _popupService;

    protected Animator _animator;
    [SerializeField]
    protected Button _btnClose;
    private CanvasGroup _cg;

    protected bool _isEnableClick;

    public Action onDestroyAction;

    protected virtual void Awake()
    {
        TryGetComponent(out _animator);

        if (_btnClose != null)
        {
            _btnClose.onClick.AddListener(OnClickClose);
        }

        _cg = GetComponent<CanvasGroup>();
    }

    public virtual void OnDestroy()
    {
        onDestroyAction?.Invoke();
    }

    public virtual void Show(object data = null)
    {
        if (_animator != null &&
            _animator.runtimeAnimatorController != null &&
            _animator.runtimeAnimatorController.name == "PopupAni")
        {
            // transform.localScale = Vector3.zero;
            if (_cg != null)
            {
                _cg.alpha = 0f;
            }
        }
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public virtual void InitDependencies(UIPopupDependencies dependencies)
    {
        _assetService = dependencies.assetService;
        _soundService = dependencies.soundService;
        _popupService = dependencies.popupService;
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
        _soundService.PlaySFX(SoundId.SFX_BTN_CLICK);
        _popupService.ClosePopup(this);
    }
}