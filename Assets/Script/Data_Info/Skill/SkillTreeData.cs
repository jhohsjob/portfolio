using UnityEngine;


[CreateAssetMenu(menuName = "GameData/SkillTreeData")]
public class SkillTreeData : ScriptableObject
{
    public int id;
    public SkillData[] projectileData;
}
