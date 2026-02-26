using UnityEngine;


[CreateAssetMenu(menuName = "GameDefinition/MercenaryData")]
public class MercenaryData : RoleData
{
    public SkillData skillData;
    public int dashCount;
    public float dashCooldown;
    public Sprite icon;
}
