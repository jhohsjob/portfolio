using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public interface IPopupService
{
    void ShowPopup<T>(string address = "", object data = null, Action<T> onLoadedCallback = null) where T : UIPopup;

    void ClosePopup(UIPopup popup);

    void CloseAllPopup();
}

public class PopupServiceDependencies
{
    public IAssetLoader AssetLoader;
    public Storage Storage;
}

public class PopupService : IPopupService
{
    private PopupServiceDependencies _context;

    private List<UIPopup> _openPopup;

    private Transform _container;
    private Transform _alphaLayer;

    public PopupService(PopupServiceDependencies context)
    {
        _context = context;

        _openPopup = new List<UIPopup>();

        BindingContainer();
    }

    private void BindingContainer()
    {
        _container = GameObject.Find("Global/PopupCanvas").transform;
        _alphaLayer = _container.Find("AlphaBlack");
    }

    public void ShowPopup<T>(string address = "", object data = null, Action<T> onLoadedCallback = null) where T : UIPopup
    {
        _context.AssetLoader.LoadPrefab(address, (prefab) =>
        {
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

            SetupPopup(popup, data);
        }, () =>
        {
            Debug.LogError("Popup load failed: " + address);
        });
    }

    private void SetupPopup(UIPopup popup, object data)
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

        popup.InitDependencies(this);
        popup.OnPopupReady(data);
        popup.Show();

        _openPopup.Add(popup);
    }

    public void ClosePopup<T>()
    {
        Type t = typeof(T);
        UIPopup Popup = GetPopup(t);
        if (Popup != null)
        {
            ClosePopup(Popup);
        }
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
        if (popup == null)
        {
            return;
        }

        if (popup.transform != null && popup.transform.TryGetComponent(out Animator animator))
        {
            var stateId = Animator.StringToHash("AutoClose");
            if (animator.HasState(0, stateId))
            {
                animator.Play(stateId);
                PlayAlphaLayerFadeOutAnim(popup);
                return;
            }
            if (animator.layerCount > 1 && animator.HasState(1, stateId))
            {
                animator.Play(stateId, 1);
                PlayAlphaLayerFadeOutAnim(popup);
                return;
            }
        }
        ClosePopupWithoutAnim(popup);
    }

    public void ClosePopupWithoutAnim(UIPopup popup)
    {
        if (popup == null)
        {
            return;
        }

        _openPopup.Remove(popup);
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
        _alphaLayer.gameObject.SetActive(false);
    }

    public UIPopup GetTopPopup()
    {
        if (_openPopup.Count > 0)
        {
            return _openPopup[_openPopup.Count - 1];
        }

        return null;
    }

    public void PlayAlphaLayerFadeOutAnim(UIPopup popup, Action callback = null)
    {
        if (popup == null) return;

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
}