using UnityEngine;


[CreateAssetMenu(menuName = "GameDefinition/SkillTreeData")]
public class SkillTreeData : ScriptableObject
{
    public int id;
    public SkillData[] projectileData;
}
