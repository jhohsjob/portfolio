using UnityEngine;


[CreateAssetMenu(menuName = "GameDefinition/MercenaryDefinition")]
public class MercenaryDefinition : RoleDefinition
{
    public SkillData skillData;
    public int dashCount;
    public float dashCooldown;
    public Sprite icon;
}
