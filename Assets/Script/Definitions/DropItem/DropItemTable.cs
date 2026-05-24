using SerializableDictionary.Scripts;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameTable/DropItemTable")]
public class DropItemTable : ScriptableObject
{
    [SerializeField]
    private SerializableDictionary<int, DropItemDefinition> _table;
    public Dictionary<int, DropItemDefinition> table => _table.Dictionary;
}
