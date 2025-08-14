using SerializableDictionary.Scripts;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameTable/MonsterTable")]
public class MonsterTable : ScriptableObject
{
    [SerializeField]
    private SerializableDictionary<int, MonsterData> _table;
    public Dictionary<int, MonsterData> table => _table.Dictionary;
}
