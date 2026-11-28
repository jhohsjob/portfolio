using SerializableDictionary.Scripts;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameTable/ProjectileDefinitionTable")]
public class ProjectileDefinitionTable : IdSyncTable
{
    [SerializeField]
    private SerializableDictionary<int, ProjectileDefinition> _table;
    public Dictionary<int, ProjectileDefinition> table => _table.Dictionary;
}
