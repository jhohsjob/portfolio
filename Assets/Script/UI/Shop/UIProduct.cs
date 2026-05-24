using System;
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
    private int _currentPurchaseCount;

    public event Action<ProductDefinition> onClickProduct;

    private void Awake()
    {
        _btn ??= GetComponent<Button>();
        _desc ??= GetComponentInChildren<TextMeshProUGUI>();

        _btn.onClick.AddListener(() =>
        {
            if (_definition != null) onClickProduct?.Invoke(_definition);
        });

        LocalizationSettings.SelectedLocaleChanged += locale => { UpdateUI(); };
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= locale => { UpdateUI(); };
    }

    public void Bind(ProductDefinition definition, int purchaseCount)
    {
        _definition = definition;
        _currentPurchaseCount = purchaseCount;

        UpdateUI();
    }

    public void UpdatePurchaseCount(int newCount)
    {
        _currentPurchaseCount = newCount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_definition == null || _desc == null) return;

        string productName = LocalizationSettings.StringDatabase.GetLocalizedString(LocalTable.ShopTable, _definition.GetNameKey());
        string price = _definition.currencyType == CurrencyType.Free ? $"{_definition.currencyType}" : $"{_definition.currencyType} {_definition.price}";

        _desc.text = $"{productName}\n{price}\n{_definition.limitType} {_currentPurchaseCount}/{_definition.maxPurchaseCount}";
    }
}