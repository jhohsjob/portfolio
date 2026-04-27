using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameDefinition/ShopItemDefinition")]
public class ShopItemDefinition : ScriptableObject
{
    public int id;
    public string shopItemName;
    public List<ProductDefinition> productItems;
    public Transform prefab;
}