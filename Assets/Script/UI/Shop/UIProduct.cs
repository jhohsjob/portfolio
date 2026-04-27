using System.Threading.Tasks;
using TMPro;
using UnityEngine;
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
    }

    public void Bind(ProductDefinition definition)
    {
        _definition = definition;

        UpdateUI();
    }

    public void UpdateUI()
    {
        var data = Client.productStorage.Get(_definition.id);
        int purchaseCount = data?.purchaseCount ?? 0;

        _desc.text = $"{_definition.productName}\n{_definition.currencyType} {_definition.price}\n{_definition.limitType} {purchaseCount}/{_definition.maxPurchaseCount}";
    }

    public async Task OnClickItem()
    {
        var (success, reason) = await PurchaseManager.instance.TryPurchase(_definition);

        if (success)
        {
            UpdateUI();

            PopupManager.ShowPopup<UIProductResult>(
                PopupName.UIProductResult,
                new UIProductResultData(true, $"You have purchased {_definition.productName}!"));
        }
        else
        {
            PopupManager.ShowPopup<UIProductResult>(
                PopupName.UIProductResult,
                new UIProductResultData(false, $"Purchase failed: {_definition.productName}\nReason: {reason.ToMessage()}"));
        }
    }
}
