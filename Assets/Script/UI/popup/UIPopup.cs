using UnityEngine;
using UnityEngine.UI;


public class UIPopup : MonoBehaviour
{
    [SerializeField]
    protected Button _btnClose;

    private void Awake()
    {
        if (_btnClose != null)
        {
            _btnClose.onClick.AddListener(OnClickClose);
        }
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

    protected virtual void OnClickClose()
    {
        Hide();
    }
}
