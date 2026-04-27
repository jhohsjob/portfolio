using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(menuName = "Reward/Gold")]
public class GoldReward : RewardBase
{
    public int amount;

    public override RewardResult CanGive()
    {
        
        return RewardResult.Success(
            RewardType.Gold,
            amount: amount,
            message: $"gold {amount}"
            );
    }

    public override Task Apply()
    {
        Client.currencyService.Change(CurrencyType.Gold, amount);
        return Task.CompletedTask;
    }
}