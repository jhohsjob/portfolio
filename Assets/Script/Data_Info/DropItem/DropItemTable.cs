using SerializableDictionary.Scripts;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameTable/DropItemTable")]
public class DropItemTable : ScriptableObject
{
    [SerializeField]
    private SerializableDictionary<int, DropItemData> _table;
    public Dictionary<int, DropItemData> table => _table.Dictionary;
}
