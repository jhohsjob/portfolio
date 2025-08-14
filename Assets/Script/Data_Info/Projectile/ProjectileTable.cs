using SerializableDictionary.Scripts;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameTable/ProjectileTable")]
public class ProjectileTable : ScriptableObject
{
    [SerializeField]
    private SerializableDictionary<int, ProjectileData> _table;
    public Dictionary<int, ProjectileData> table => _table.Dictionary;
}
