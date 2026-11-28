using TMPro;
using UnityEngine;


public class UICommonPopupData
{
    public string title;
    public string message;
}

public class UICommonPopup : UIPopup
{
    [SerializeField]
    private TextMeshProUGUI _txtTitle;
    [SerializeField]
    private TextMeshProUGUI _txtMessage;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnPopupReady(object data = null)
    {
        if (data is not UICommonPopupData popupData)
        {
            return;
        }

        _txtTitle.text = popupData.title;
        _txtMessage.text = popupData.message;

        base.OnPopupReady(data);
    }
}