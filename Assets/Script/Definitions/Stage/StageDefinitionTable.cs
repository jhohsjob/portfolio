using SerializableDictionary.Scripts;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameTable/StageDefinitionTable")]
public class StageDefinitionTable : ScriptableObject
{
    [SerializeField]
    private SerializableDictionary<int, StageDefinition> _table;
    public Dictionary<int, StageDefinition> table => _table.Dictionary;
}
