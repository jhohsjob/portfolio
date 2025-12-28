using UnityEngine;


public class UILobbyMiddle : MonoBehaviour
{
    protected virtual void Awake()
    {

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
}
