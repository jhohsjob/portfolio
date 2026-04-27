using TMPro;
using UnityEngine;


public class UIProductResultData
{
    public bool success;
    public string message;
    public UIProductResultData(bool success, string message)
    {
        this.success = success;
        this.message = message;
    }
}

public class UIProductResult : UIPopup
{
    [SerializeField]
    private TextMeshProUGUI _txtResult;
    [SerializeField]
    private TextMeshProUGUI _txtMessage;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnPopupReady(object data = null)
    {
        if (data is not UIProductResultData popupData)
        {
            return;
        }

        _txtResult.text = popupData.success ? "Purchase Successful!" : "Purchase Failed!";
        _txtMessage.text = popupData.message;

        base.OnPopupReady(data);
    }
}