using UnityEngine;


public abstract class UILobbyMiddleBase : MonoBehaviour
{
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
