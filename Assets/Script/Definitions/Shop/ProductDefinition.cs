using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameDefinition/ProductDefinition")]
public class ProductDefinition : ScriptableObject
{
    public int id;
    public string productName;
    public ShopLimitType limitType;
    public int maxPurchaseCount;
    public CurrencyType currencyType;
    public int price;
    public List<RewardBase> rewards;
}