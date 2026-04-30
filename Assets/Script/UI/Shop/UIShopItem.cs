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

    public void Awake()
    {
        EventHelper.AddEventListener(EventName.LocaleChanged, (sender, data) => UpdateUI());
    }

    private void OnDestroy()
    {
        EventHelper.RemoveEventListener(EventName.LocaleChanged, (sender, data) => UpdateUI());
    }

    public void Bind(ShopItemDefinition definition)
    {
        _definition = definition;

        int count = Mathf.Min(_items.Count, definition.productItems.Count);

        for (int i = 0; i < count; i++)
        {
            _items[i].Bind(definition.productItems[i]);
            _items[i].gameObject.SetActive(true);
        }

        for (int i = count; i < _items.Count; i++)
        {
            _items[i].gameObject.SetActive(false);
        }

        if (definition.productItems.Count > _items.Count)
        {
            Debug.LogWarning($"[{name}] UIProduct ºÎÁ·: {definition.productItems.Count} > {_items.Count}");
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_txtTitle != null)
        {
            _txtTitle.text = LocalizationSettings.StringDatabase.GetLocalizedString(LocalTable.ShopTable, _definition.GetTitleKey()) ?? string.Empty;
        }
    }
}
