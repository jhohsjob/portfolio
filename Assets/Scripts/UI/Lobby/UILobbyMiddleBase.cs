using UnityEngine;


public abstract class UILobbyMiddleBase : MonoBehaviour
{
    protected virtual void Awake()
    {
    }

    private void OnDestroy()
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
