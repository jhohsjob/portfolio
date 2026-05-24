using UnityEngine;


[CreateAssetMenu(menuName = "Reward/Mercenary")]
public class MercenaryReward : RewardBase
{
    public int mercenaryId;

    public override RewardResult CanGive()
    {
        var mercenary = MercenaryManager.instance.GetMercenaryById(mercenaryId);
        if (mercenary == null)
        {
            return RewardResult.Fail("id error");
        }

        if (mercenary.isOwned == true)
        {
            return RewardResult.Fail("has mercenary");
        }

        return RewardResult.Success(
                RewardType.Mercenary,
                id: mercenaryId,
                message: $"Mercenary {mercenaryId} acquired"
            );
    }
}