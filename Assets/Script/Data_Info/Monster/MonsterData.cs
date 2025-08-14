using UnityEngine;


[CreateAssetMenu(menuName = "GameData/MonsterData")]
public class MonsterData : RoleData
{
    public float maxHP;

    public override void Init(RoleData data)
    {
        base.Init(data);

        if (data is MonsterData actorData)
        {
            maxHP = actorData.maxHP;
        }
    }
}
