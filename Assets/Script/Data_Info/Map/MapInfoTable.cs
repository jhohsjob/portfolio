using SerializableDictionary.Scripts;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameTable/MapInfoTable")]
public class MapInfoTable : ScriptableObject
{
    [SerializeField]
    private SerializableDictionary<int, MapInfoData> _table;
    public Dictionary<int, MapInfoData> table => _table.Dictionary;
}
