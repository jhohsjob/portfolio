using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameTable/ShopItemDefinitionTable")]
public class ShopItemDefinitionTable : ScriptableObject
{
    [SerializeField]
    private List<ShopItemDefinition> _table;
    public IReadOnlyList<ShopItemDefinition> table => _table;
}