using SerializableDictionary.Scripts;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameTable/SkillTable")]
public class SkillTable : ScriptableObject
{
    [SerializeField]
    private SerializableDictionary<int, SkillData> _table;
    public Dictionary<int, SkillData> table => _table.Dictionary;
}
