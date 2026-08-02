using UnityEngine;


[CreateAssetMenu(menuName = "GameDefinition/MercenaryDefinition")]
public class MercenaryDefinition : RoleDefinition
{
    public SkillTreeDefinition skillTreeDefinition;
    public int dashCount;
    public float dashCooldown;
    public Sprite icon;
}
