using UnityEngine;


[CreateAssetMenu(menuName = "GameData/MercenaryData")]
public class MercenaryData : RoleData
{
    public SkillData skillData;
    public int dashCount;
    public float dashCooldown;
}
