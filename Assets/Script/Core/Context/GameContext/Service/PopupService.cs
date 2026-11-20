using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public interface IPopupService : IDisposable
{
    void ShowPopup<T>(string address = "", object data = null, UIPopupDependencies dependencies = null, Action<T> onLoadedCallback = null) where T : UIPopup;
    void ShowCommonPopup(string title, string message);

    void ClosePopup(UIPopup popup);
    void CloseAllPopup();
}

public class PopupServiceDependencies
{
    public IAssetService assetService;
    public ISoundService soundService;
    public Transform container;
}

public class PopupService : IPopupService
{
    private PopupServiceDependencies _context;

    private List<UIPopup> _openPopup = new();
    private readonly HashSet<UIPopup> _closing = new();
    private readonly CancellationTokenSource _cts = new();

    private Transform _container;
    private Transform _alphaLayer;
    private bool _disposed;

    public PopupService(PopupServiceDependencies context)
    {
        _context = context;

        _container = context.container;
        _alphaLayer = _container.Find("AlphaBlack");
    }

    public void ShowPopup<T>(string address = "", object data = null, UIPopupDependencies dependencies = null, Action<T> onLoadedCallback = null) where T : UIPopup
    {
        if (_disposed == true)
        {
            return;
        }

        _context.assetService.LoadPrefab(address, (prefab) =>
        {
            if (_disposed == true)
            {
                return;
            }

            GameObject go = UnityEngine.Object.Instantiate(prefab);
            go.transform.SetParent(_container, false);
            go.name = address;

            T popup = go.GetComponent<T>();
            if (popup == null)
            {
                Debug.LogError($"Prefab {address} does not contain component {typeof(T).Name}");
                UnityEngine.Object.Destroy(go);
                return;
            }

            onLoadedCallback?.Invoke(popup);

            SetupPopup(popup, data, dependencies);
        }, () =>
        {
            Debug.LogError("Popup load failed: " + address);
        });
    }

    private void SetupPopup(UIPopup popup, object data, UIPopupDependencies dependencies)
    {
        popup.Hide();

        // Alpha mask setting
        if (_openPopup.Count == 0)
        {
            _alphaLayer.gameObject.SetActive(true);
            var maskImage = _alphaLayer.GetComponent<Image>();
            maskImage.DOKill();
            maskImage.DOFade(0f, 0.4f).From();
            maskImage.DOFade(0.8f, 0.4f);
        }

        dependencies ??= new UIPopupDependencies();

        dependencies.assetService = _context.assetService;
        dependencies.soundService = _context.soundService;
        dependencies.popupService = this;

        popup.InitDependencies(dependencies);
        popup.OnPopupReady(data);
        popup.Show();

        _openPopup.Add(popup);
    }

    public void ClosePopup<T>()
    {
        ClosePopup(typeof(T));
    }

    public void ClosePopup(Type t)
    {
        UIPopup Popup = GetPopup(t);
        if (Popup != null)
        {
            ClosePopup(Popup);
        }
    }

    public void ClosePopup(UIPopup popup)
    {
        if (_disposed == true || popup == null)
        {
            return;
        }

        if (_closing.Add(popup) == false)
        {
            return;
        }

        if (popup.TryGetComponent(out Animator animator))
        {
            ClosePopupWithAnimAsync(popup, animator, _cts.Token).Forget();
        }
        else
        {
            ClosePopupWithoutAnim(popup);
        }
    }

    private async UniTaskVoid ClosePopupWithAnimAsync(UIPopup popup, Animator animator, CancellationToken ct)
    {
        try
        {
            var stateId = Animator.StringToHash("AutoClose");
            if (animator.HasState(0, stateId))
            {
                animator.Play(stateId);

                await UniTask.Yield(ct);

                while (animator != null &&
                       animator.GetCurrentAnimatorStateInfo(0).shortNameHash == stateId &&
                       animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                {
                    await UniTask.Yield(ct);
                }
            }

            if (popup != null)
            {
                PlayAlphaLayerFadeOutAnim(popup);
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            _closing.Remove(popup);
        }
    }

    public void ClosePopupWithoutAnim(UIPopup popup)
    {
        if (popup == null)
        {
            return;
        }

        _openPopup.Remove(popup);
        _closing.Remove(popup);
        GameObject.Destroy(popup.gameObject);

        if (_openPopup.Count > 0)
        {
            _openPopup[_openPopup.Count - 1].transform.parent.SetAsLastSibling();
            var maskImage = _alphaLayer.GetComponent<Image>();
            maskImage.DOKill();
            maskImage.color = new Color(0f, 0f, 0f, 0.8f);
        }
        else
        {
            _alphaLayer.gameObject.SetActive(false);
        }
    }

    public UIPopup GetPopup(Type type)
    {
        for (var i = 0; i < _openPopup.Count; i++)
        {
            if (_openPopup[i].GetType() == type)
            {
                return _openPopup[i];
            }
        }

        return null;
    }

    public T GetPopup<T>() where T : UIPopup => GetPopup(typeof(T)) as T;
    public void CloseAllPopup()
    {
        for (var i = _openPopup.Count - 1; i >= 0; i--)
        {
            if (i >= _openPopup.Count)
            {
                continue;
            }

            var popup = _openPopup[i];
            _openPopup.RemoveAt(i);
            GameObject.Destroy(popup.gameObject);
        }

        _openPopup.Clear();
        _closing.Clear();

        if (_alphaLayer != null)
        {
            _alphaLayer.gameObject.SetActive(false);
        }
    }

    public UIPopup GetTopPopup()
    {
        if (_openPopup.Count > 0)
        {
            return _openPopup[_openPopup.Count - 1];
        }

        return null;
    }

    private void PlayAlphaLayerFadeOutAnim(UIPopup popup, Action callback = null)
    {
        if (popup == null)
        {
            return;
        }

        if (_alphaLayer.gameObject.activeSelf && _alphaLayer.TryGetComponent(out Image image))
        {
            if (_openPopup.Count == 1)
            {
                image.DOKill();
                image.DOFade(0.8f, 1f / 3f).From();
                image.DOFade(0f, 1f / 3f).SetDelay(1f / 3f);
            }
        }
        var rect = popup.GetComponent<RectTransform>();
        if (rect != null && rect.localScale.x != 0 && rect.localScale.y != 0 && (rect.localScale.x != 1 || rect.localScale.y != 1))
        {
            var size = _container.GetComponent<RectTransform>().sizeDelta;
            rect.sizeDelta = new Vector2(size.x / rect.localScale.x, size.y / rect.localScale.y);
        }

        ClosePopupWithoutAnim(popup);
        callback?.Invoke();
    }

    public bool IsPopupTop(UIPopup Popup)
    {
        if (Popup == null)
        {
            return false;
        }

        if (_openPopup.Count <= 1)
        {
            return true;
        }

        return GetTopPopup() == Popup;
    }

    public void ShowAlphaBlack(bool show)
    {
        _alphaLayer.gameObject.SetActive(show);
    }

    public void ShowCommonPopup(string title, string message)
    {
        var data = new UICommonPopupData
        {
            title = title,
            message = message
        };
        ShowPopup<UICommonPopup>(PopupName.UICommonPopup, data);
    }

    public void Dispose()
    {
        if (_disposed == true)
        {
            return;
        }
        _disposed = true;

        _cts.Cancel();
        _cts.Dispose();
           
        if (_alphaLayer != null && _alphaLayer.TryGetComponent(out Image image))
        {
            image.DOKill();
        }

        CloseAllPopup();
    }
}