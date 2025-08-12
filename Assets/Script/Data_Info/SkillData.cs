using UnityEngine;


[CreateAssetMenu(menuName = "GameData/SkillData")]
public class SkillData : ScriptableObject
{
    public int id;
    public string skillName;
    public string behaviourName;

    public int projectileId;

    public int shotCount;
    public float shotDelay;
    public float reloadTime;
}
