using SerializableDictionary.Scripts;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameTable/ProductDefinitionTable")]
public class ProductDefinitionTable : ScriptableObject
{
    [SerializeField]
    private SerializableDictionary<int, ProductDefinition> _table;
    public Dictionary<int, ProductDefinition> table => _table.Dictionary;
}
