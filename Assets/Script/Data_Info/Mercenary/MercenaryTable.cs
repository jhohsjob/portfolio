using SerializableDictionary.Scripts;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameTable/MercenaryTable")]
public class MercenaryTable : ScriptableObject
{
    [SerializeField]
    private SerializableDictionary<int, MercenaryData> _table;
    public Dictionary<int, MercenaryData> table => _table.Dictionary;
}
