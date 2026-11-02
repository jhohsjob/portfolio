using TMPro;
using UnityEngine;


public class UIProductResultData
{
    public PurchaseResult result;
    public string message;
}

public class UIProductResultPopup : UIPopup
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

        string key = popupData.result == PurchaseResult.Success ? "success_result_tile" : "failed_result_title";
        _txtResult.text = LocalManager.GetShopString(key);
        _txtMessage.text = popupData.message;

        base.OnPopupReady(data);
    }
}