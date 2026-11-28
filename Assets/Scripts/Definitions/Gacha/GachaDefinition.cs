using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameDefinition/GachaDefinition")]
public class GachaDefinition : DefinitionBase
{
    public PurchaseType purchaseType;
    public CurrencyType currencyType;
    public int price;
    public int[] drawCount;

    public List<GachaRewardDefinition> rewards;

    public GameObject scrollUI;
    public GameObject mainUI;
}

[Serializable]
public class GachaRewardDefinition
{
    public RewardBase reward;

    public float weight;
    public float probability;
}