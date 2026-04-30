using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;


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

        string key = popupData.success ? "success_result_tile" : "failed_result_title";
        _txtResult.text = LocalizationSettings.StringDatabase.GetLocalizedString(LocalTable.ShopTable, key);
        _txtMessage.text = popupData.message;

        base.OnPopupReady(data);
    }
}