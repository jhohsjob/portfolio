using UnityEngine;


public class UIPanel : MonoBehaviour
{
    protected virtual void Awake()
    {
    }

    protected virtual void OnDestroy()
    {
    }

    protected virtual void Init(object data = null)
    {

    }

    protected virtual void Clear()
    {

    }

    public virtual void Show(object data = null)
    {
        Init(data);

        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        Clear();

        gameObject.SetActive(false);
    }
}
