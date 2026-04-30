using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;


public class UIProduct : MonoBehaviour
{
    [SerializeField]
    private Button _btn;
    [SerializeField]
    private TextMeshProUGUI _desc;

    private ProductDefinition _definition;

    private void Awake()
    {
        _btn ??= GetComponent<Button>();
        _desc ??= GetComponentInChildren<TextMeshProUGUI>();

        _btn.onClick.AddListener(() => _ = OnClickItem());

        EventHelper.AddEventListener(EventName.LocaleChanged, (sender, data) => UpdateUI());
    }

    private void OnDestroy()
    {
        EventHelper.RemoveEventListener(EventName.LocaleChanged, (sender, data) => UpdateUI());
    }

    public void Bind(ProductDefinition definition)
    {
        _definition = definition;

        UpdateUI();
    }

    private void UpdateUI()
    {
        var data = Client.productStorage.Get(_definition.id);
        int purchaseCount = data?.purchaseCount ?? 0;

        string productName = LocalizationSettings.StringDatabase.GetLocalizedString(LocalTable.ShopTable, _definition.GetNameKey());
        string price = _definition.currencyType == CurrencyType.Free ? $"{_definition.currencyType}" : $"{_definition.currencyType} {_definition.price}";
        _desc.text = $"{productName}\n{price}\n{_definition.limitType} {purchaseCount}/{_definition.maxPurchaseCount}";
    }

    public async Task OnClickItem()
    {
        var (success, reason) = await PurchaseManager.instance.TryPurchase(_definition);

        string productName = LocalizationSettings.StringDatabase.GetLocalizedString(LocalTable.ShopTable, _definition.GetNameKey());
        string message = string.Empty;
        if (success)
        {
            UpdateUI();

            message = LocalizationSettings.StringDatabase.GetLocalizedString(
                LocalTable.ShopTable,
                "success_result_message",
                new object[] { new { product_name = productName } });
        }
        else
        {
            message = LocalizationSettings.StringDatabase.GetLocalizedString(
                LocalTable.ShopTable,
                "failed_result_message",
                new object[] { new { product_name = productName, reason = reason.ToMessage() } });
        }

        PopupManager.ShowPopup<UIProductResultPopup>(PopupName.UIProductResult, new UIProductResultData(success, message));
    }
}
