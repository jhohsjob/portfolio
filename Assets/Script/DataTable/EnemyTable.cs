using SerializableDictionary.Scripts;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameTable/EnemyTable")]
public class EnemyTable : ScriptableObject
{
    [SerializeField]
    private SerializableDictionary<int, EnemyData> _table;
    public Dictionary<int, EnemyData> table => _table.Dictionary;
}
