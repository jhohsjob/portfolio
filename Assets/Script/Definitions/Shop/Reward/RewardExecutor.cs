using System.Threading.Tasks;


public class RewardExecutor
{
    private readonly MercenaryService _mercenaryService;
    private readonly ICurrencyService _currency;

    public RewardExecutor(MercenaryService mercenaryService, ICurrencyService currencyService)
    {
        _mercenaryService = mercenaryService;
        _currency = currencyService;
    }

    public Task Apply(RewardBase reward)
    {
        switch (reward)
        {
            case GoldReward gold:
                _currency.Change(CurrencyType.Gold, gold.amount);
                return Task.CompletedTask;

            case MercenaryReward mercenary:
                return _mercenaryService.Acquire(mercenary.mercenaryId);

            default:
                return Task.CompletedTask;
        }
    }
}