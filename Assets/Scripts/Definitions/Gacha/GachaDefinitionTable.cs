using SerializableDictionary.Scripts;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameTable/GachaDefinitionTable")]
public class GachaDefinitionTable : IdSyncTable
{
    [SerializeField]
    private SerializableDictionary<int, GachaDefinition> _table;
    public Dictionary<int, GachaDefinition> table => _table.Dictionary;
}
