using SerializableDictionary.Scripts;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameTable/SoundDefinitionTable")]
public class SoundDefinitionTable : IdSyncTable
{
    [SerializeField]
    private SerializableDictionary<int, SoundDefinition> _table;
    public Dictionary<int, SoundDefinition> table => _table.Dictionary;
}