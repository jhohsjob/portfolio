using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;


public class UIProduct : MonoBehaviour
{
    private PurchaseService _purchaseService;

    [SerializeField]
    private Button _btn;
    [SerializeField]
    private TextMeshProUGUI _desc;

    private ProductDefinition _definition;

    public event Action<ProductDefinition> onClickProduct;

    private void Awake()
    {
        _btn ??= GetComponent<Button>();
        _desc ??= GetComponentInChildren<TextMeshProUGUI>();

        _btn.onClick.AddListener(() =>
        {
            if (_definition != null)
            {
                onClickProduct?.Invoke(_definition);
            }
        });

        LocalizationSettings.SelectedLocaleChanged += HandleLocaleChanged;
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= HandleLocaleChanged;

        if (_purchaseService != null)
        {
            _purchaseService.onPVOChanged -= HandlePVOChanged;
        }
    }

    public void Bind(PurchaseService purchaseService, ProductDefinition definition)
    {
        _purchaseService = purchaseService;
        _definition = definition;

        UpdateUI();

        if (_purchaseService != null)
        {
            _purchaseService.onPVOChanged -= HandlePVOChanged;
        }
        _purchaseService.onPVOChanged += HandlePVOChanged;
    }

    private void UpdateUI()
    {
        if (_definition == null || _desc == null)
        {
            return;
        }

        string productName = LocalizationHelper.GetShopString(_definition.GetNameKey());
        string price = string.Empty;

        if (_definition.purchaseType == PurchaseType.Free)
        {
            price = $"{_definition.currencyType}";
        }
        else if (_definition.purchaseType == PurchaseType.IAP)
        {
            price = $"$ {_definition.price}";
        }
        else
        {
            price = $"{_definition.currencyType} {_definition.price}";
        }

        string desc = string.Empty;

        switch (_definition.limitType)
        {
            case ShopLimitType.None:
                _desc.text = $"{productName}\n{price}";
                break;

            case ShopLimitType.Daily:
            case ShopLimitType.Lifetime:
                _desc.text = $"{productName}\n{price}\n{_definition.limitType}" +
                    $" {_purchaseService.GetProductPVOById(_definition.Id).purchaseCount}/{_definition.purchaseLimit}";
                break;
        }
    }

    private void HandleLocaleChanged(Locale locale)
    {
        UpdateUI();
    }

    private void HandlePVOChanged(ProductPVO pvo)
    {
        UpdateUI();
    }
}