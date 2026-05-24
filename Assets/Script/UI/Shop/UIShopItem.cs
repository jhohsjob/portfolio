using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;


public class UIShopItem : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _txtTitle;
    [SerializeField]
    private List<UIProduct> _items = new();

    private ShopItemDefinition _definition;

    public event Action<ShopItemDefinition, ProductDefinition, UIProduct> onProductClick;

    private void Awake()
    {
        LocalizationSettings.SelectedLocaleChanged += locale => { UpdateUI(); };
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= locale => { UpdateUI(); };
    }

    public void Bind(ShopItemDefinition definition, Func<int, int> getPurchaseCountFunc)
    {
        _definition = definition;
        if (_definition == null)
        {
            return;
        }

        int count = Mathf.Min(_items.Count, definition.productItems.Count);

        for (int i = 0; i < count; i++)
        {
            var productDefinition = definition.productItems[i];
            int currentCount = getPurchaseCountFunc?.Invoke(productDefinition.id) ?? 0;

            _items[i].Bind(productDefinition, currentCount);
            _items[i].gameObject.SetActive(true);

            var currentProductUi = _items[i];
            currentProductUi.onClickProduct += (def) =>
            {
                onProductClick?.Invoke(_definition, def, currentProductUi);
            };
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
            _txtTitle.text = LocalizationSettings.StringDatabase.GetLocalizedString(LocalTable.ShopTable, _definition.GetTitleKey()) ?? string.Empty;
        }
    }
}