using UnityEngine;


[CreateAssetMenu(menuName = "GameData/MercenaryData")]
public class MercenaryData : RoleData
{
    public float maxHP;

    public override void Init(RoleData data)
    {
        base.Init(data);

        if (data is MercenaryData actorData)
        {
            maxHP = actorData.maxHP;
        }
    }
}
