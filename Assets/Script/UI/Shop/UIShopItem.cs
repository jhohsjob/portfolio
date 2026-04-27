using System.Collections.Generic;
using UnityEngine;


public class UIShopItem : MonoBehaviour
{
    [SerializeField]
    private List<UIProduct> _items = new();

    public void Bind(ShopItemDefinition definition)
    {
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
    }
}
