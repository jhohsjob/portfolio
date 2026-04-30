using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameDefinition/ProductDefinition")]
public class ProductDefinition : ScriptableObject
{
    public int id;
    public ShopLimitType limitType;
    public int maxPurchaseCount;
    public CurrencyType currencyType;
    public int price;
    public List<RewardBase> rewards;

    private string LocalKey(string prefix) => $"{prefix}_{id}";

    public string GetNameKey()
    {
        return LocalKey("product_name");
    }
}