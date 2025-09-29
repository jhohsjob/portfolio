using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PopupManager
{
    private static List<UIPopup> _openPopup;

    private static Transform _container;
    private static Transform _alphaLayer;

    private static PopupManager _instance;

    public PopupManager()
    {
        _openPopup = new List<UIPopup>();
    }

    public static void Initialization()
    {
        if (_instance == null)
        {
            _instance = new PopupManager();
        }

        _instance.BindingContainer();
    }

    private void BindingContainer()
    {
        _container = GameObject.Find("Global/PopupCanvas").transform;
        _alphaLayer = _container.Find("AlphaBlack");
    }

    public static void ShowPopup<T>(string address = "", object data = null, Action<UIPopup> onLoadedCallback = null) where T : UIPopup
    {
        if (Client.isRunGame == false)
        {
            Debug.Log("Wait for the game to run");
            return;
        }

        Client.asset.LoadAsset<GameObject>(address, (task) =>
        {
            GameObject prefab = task.GetAsset<GameObject>();
            if (prefab == null)
            {
                Debug.LogError("Popup prefab load failed: " + address);
                return;
            }

            GameObject go = UnityEngine.Object.Instantiate(prefab, _container);
            go.name = address;

            T popup = go.GetComponent<T>();
            if (popup == null)
            {
                Debug.LogError($"Prefab {address} does not contain component {typeof(T).Name}");
                UnityEngine.Object.Destroy(go);
                return;
            }

            SetupPopup(popup, data, onLoadedCallback);
        }, () =>
        {
            Debug.LogError("Popup load failed: " + address);
        });
    }

    private static void SetupPopup(UIPopup popup, object data, Action<UIPopup> onLoadedCallback)
    {
        var rect = popup.GetComponent<RectTransform>();
        rect.sizeDelta = _container.GetComponent<RectTransform>().sizeDelta;

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

        onLoadedCallback?.Invoke(popup);

        popup.OnPopupReady(data);
        popup.Show();

        _openPopup.Add(popup);
    }

    public static void ClosePopup<T>()
    {
        Type t = typeof(T);
        UIPopup Popup = GetPopup(t);
        if (Popup != null)
        {
            ClosePopup(Popup);
        }
    }

    public static void ClosePopup(Type t)
    {
        UIPopup Popup = GetPopup(t);
        if (Popup != null)
        {
            ClosePopup(Popup);
        }
    }

    public static void ClosePopup(UIPopup popup)
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

    public static void ClosePopupWithoutAnim(UIPopup popup)
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

    public static UIPopup GetPopup(Type type)
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

    public static T GetPopup<T>() where T : UIPopup => GetPopup(typeof(T)) as T;
    public static void CloseAllPopup()
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

    public static UIPopup GetTopPopup()
    {
        if (_openPopup.Count > 0)
        {
            return _openPopup[_openPopup.Count - 1];
        }

        return null;
    }

    public static void PlayAlphaLayerFadeOutAnim(UIPopup popup, Action callback = null)
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

    public static bool IsPopupTop(UIPopup Popup)
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

    public static void ShowAlphaBlack(bool show)
    {
        _alphaLayer.gameObject.SetActive(show);
    }
}