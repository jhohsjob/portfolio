using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;


public class UIShopItem : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _txtTitle;
    [SerializeField]
    private List<UIProduct> _items = new();

    private ShopItemDefinition _definition;

    public event Action<ProductDefinition> onProductClick;

    private void Awake()
    {
        LocalizationSettings.SelectedLocaleChanged += HandleLocaleChanged;

        foreach (var item in _items)
        {
            item.onClickProduct += HandleProductClick;
        }
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= HandleLocaleChanged;

        foreach (var item in _items)
        {
            item.onClickProduct += HandleProductClick;
        }
    }

    public void Bind(PurchaseService purchaseService, ShopItemDefinition definition)
    {
        if (definition == null)
        {
            return;
        }
        _definition = definition;

        int count = Mathf.Min(_items.Count, definition.productItems.Count);

        for (int i = 0; i < count; i++)
        {
            _items[i].Bind(purchaseService, definition.productItems[i]);
            _items[i].gameObject.SetActive(true);
        }

        for (int i = count; i < _items.Count; i++)
        {
            _items[i].gameObject.SetActive(false);
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_txtTitle != null && _definition != null)
        {
            _txtTitle.text = LocalManager.GetShopString(_definition.GetTitleKey());
        }
    }

    private void HandleLocaleChanged(Locale locale)
    {
        UpdateUI();
    }

    private void HandleProductClick(ProductDefinition definition)
    {
        onProductClick?.Invoke(definition);
    }
}