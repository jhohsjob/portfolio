using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameDefinition/ShopItemDefinition")]
public class ShopItemDefinition : ScriptableObject
{
    public int id;
    public List<ProductDefinition> productItems;
    public Transform prefab;

    private string LocalKey(string prefix) => $"{prefix}_{id}";

    public string GetTitleKey()
    {
        return LocalKey("shop_item_title");
    }
}