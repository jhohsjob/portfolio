using SerializableDictionary.Scripts;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameTable/ProjectileTable")]
public class ProjectileTable : ScriptableObject
{
    [SerializeField]
    private SerializableDictionary<int, ProjectileDefinition> _table;
    public Dictionary<int, ProjectileDefinition> table => _table.Dictionary;
}
