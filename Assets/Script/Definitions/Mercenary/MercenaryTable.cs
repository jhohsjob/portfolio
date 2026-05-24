using SerializableDictionary.Scripts;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameTable/MercenaryTable")]
public class MercenaryTable : ScriptableObject
{
    [SerializeField]
    private SerializableDictionary<int, MercenaryDefinition> _table;
    public Dictionary<int, MercenaryDefinition> table => _table.Dictionary;
}
