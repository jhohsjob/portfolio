public class ActorData : RoleData
{
    public float maxHP;

    public override void Init(RoleData data)
    {
        base.Init(data);

        if (data is ActorData actorData)
        {
            maxHP = actorData.maxHP;
        }
    }
}
