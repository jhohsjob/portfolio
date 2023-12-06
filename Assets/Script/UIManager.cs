using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class UIManager : MonoSingleton<UIManager>
{
    private GameObject _popupContainer = null;
    private Dictionary<string, UIPopup> _popupPool = new Dictionary<string, UIPopup>();

    protected override void OnAwake()
    {
        Debug.Log(this.GetType().Name + "." + MethodBase.GetCurrentMethod().Name);

        _popupContainer = GameObject.Find("popup");
    }

    public void ShowPopup(string popupName, object data = null)
    {
        UIPopup popup = null;

        if (_popupPool.ContainsKey(popupName))
        {
            popup = _popupPool[popupName];
        }
        else
        {
            popup = Instantiate(Resources.Load<UIPopup>("UI/Popup/" + popupName), _popupContainer.transform, false);
            if (popup != null)
            {
                //popup.transform.localScale = Vector3.one;
                //popup.transform.localPosition = Vector3.zero;
                popup.gameObject.SetActive(false);

                _popupPool.Add(popupName, popup);
            }
        }

        popup?.Show(data);
    }

    public void PopupClear()
    {
        foreach (var popup in _popupPool.Values)
        {
            GameObject.Destroy(popup.gameObject);
        }

        _popupPool.Clear();
    }
}
