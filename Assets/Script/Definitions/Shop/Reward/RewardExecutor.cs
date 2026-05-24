using System.Threading.Tasks;


public class RewardExecutor
{
    private readonly ICurrencyService _currency;

    public RewardExecutor(ICurrencyService currency)
    {
        _currency = currency;
    }

    public Task Apply(RewardBase reward)
    {
        switch (reward)
        {
            case GoldReward gold:
                _currency.Change(CurrencyType.Gold, gold.amount);
                return Task.CompletedTask;

            case MercenaryReward mercenary:
                return MercenaryManager.instance.Acquire(mercenary.mercenaryId);

            default:
                return Task.CompletedTask;
        }
    }
}