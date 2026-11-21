using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameDefinition/ProductDefinition")]
public class ProductDefinition : DefinitionBase
{
    public ShopLimitType limitType;
    public int purchaseLimit;
    public PurchaseType purchaseType;
    public CurrencyType currencyType;
    public int price;
    public List<RewardBase> rewards;

    private string LocalKey(string prefix) => $"{prefix}_{Id}";

    public string GetNameKey()
    {
        return LocalKey("product_name");
    }
}