using UnityEngine;


[CreateAssetMenu(menuName = "GameData/ProjectileData")]
public class ProjectileData : RoleData
{
    public float power;
    public float distance;
    public bool beHitDie = true;
    public ProjectileMoveType moveType;
}
