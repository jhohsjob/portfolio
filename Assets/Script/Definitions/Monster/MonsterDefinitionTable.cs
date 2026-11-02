using SerializableDictionary.Scripts;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameTable/MonsterDefinitionTable")]
public class MonsterDefinitionTable : IdSyncTable
{
    [SerializeField]
    private SerializableDictionary<int, MonsterDefinition> _table;
    public Dictionary<int, MonsterDefinition> table => _table.Dictionary;
}
