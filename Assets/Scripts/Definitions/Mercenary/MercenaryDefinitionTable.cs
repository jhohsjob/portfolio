using SerializableDictionary.Scripts;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameTable/MercenaryDefinitionTable")]
public class MercenaryDefinitionTable : IdSyncTable
{
    [SerializeField]
    private SerializableDictionary<int, MercenaryDefinition> _table;
    public Dictionary<int, MercenaryDefinition> table => _table.Dictionary;
}
