using SerializableDictionary.Scripts;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameTable/SkillTable")]
public class SkillTable : ScriptableObject
{
    [SerializeField]
    private SerializableDictionary<int, SkillDefinition> _table;
    public Dictionary<int, SkillDefinition> table => _table.Dictionary;
}
