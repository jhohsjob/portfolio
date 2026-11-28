using UnityEngine;


[CreateAssetMenu(menuName = "GameDefinition/ProjectileDefinition")]
public class ProjectileDefinition : RoleDefinition
{
    public float power;
    public float distance;
    public bool beHitDie = true;
}
