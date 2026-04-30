using UnityEngine;


public class UILobbyMiddle : MonoBehaviour
{
    protected virtual void Awake()
    {
        EventHelper.AddEventListener(EventName.LocaleChanged, OnLocaleChanged);
    }

    private void OnDestroy()
    {
        EventHelper.RemoveEventListener(EventName.LocaleChanged, OnLocaleChanged);
    }

    public void Show()
    {
        OnEnter();

        gameObject.SetActive(true);

        OnShow();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    protected virtual void OnEnter()
    {

    }

    protected virtual void OnShow()
    {

    }

    protected virtual void OnLocaleChanged(object sender, object data)
    {

    }
}
