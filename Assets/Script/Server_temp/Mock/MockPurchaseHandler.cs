using System.Threading.Tasks;


public class MockPurchaseHandler
{
    private MockPurchaseService _purchaseService;
    private MockUserService _userService;

    public MockPurchaseHandler(MockPurchaseService purchase, MockUserService user)
    {
        _purchaseService = purchase;
        _userService = user;
    }

    public async Task<PurchaseResponse> PurchaseAsync(PurchaseRequest request)
    {
        PurchaseResponse response = new();

        var product = _purchaseService.GetProductDefinitionById(request.productId);
        if (product == null)
        {
            response.result = NetworkResult.E_PURCHASE_INVALID_ID;
            return response;
        }

        var pvo = _purchaseService.GetProductPVOById(product.id);
        if (pvo == null)
        {
            response.result = NetworkResult.E_PURCHASE_INVALID;
            return response;
        }

        switch (product.purchaseType)
        {
            case PurchaseType.Free:
                break;

            case PurchaseType.Currency:
                var checkCurrency = CheckCurrency(product);
                if (checkCurrency != NetworkResult.SUCCESS)
                {
                    response.result = checkCurrency;
                    return response;
                }
                break;

            case PurchaseType.IAP:
                // todo : 영수증 처리?
                break;
        }

        var checkLimit = CheckLimit(product, pvo);
        if (checkLimit != NetworkResult.SUCCESS)
        {
            response.result = checkLimit;
            return response;
        }

        if (product.purchaseType == PurchaseType.Currency)
        {
            switch (product.currencyType)
            {
                case CurrencyType.Gold:
                    await _userService.ChangeGold(-product.price);
                    break;

                case CurrencyType.Gem:
                    await _userService.ChangeGem(-product.price);
                    break;
            }
        }

        foreach (var reward in product.rewards)
        {
            await GrantReward(reward);
        }

        pvo.purchaseCount++;
        pvo.totalPurchaseCount++;

        response.product = pvo;
        response.user = _userService.GetUser();

        return response;
    }

    private NetworkResult CheckCurrency(ProductDefinition product)
    {
        NetworkResult result = NetworkResult.SUCCESS;

        switch (product.currencyType)
        {
            case CurrencyType.Gold:
                if (_userService.gold < product.price)
                {
                    result = NetworkResult.E_PURCHASE_NOT_ENOUGH_GOLD;
                }
                break;

            case CurrencyType.Gem:
                if (_userService.gem < product.price)
                {
                    result = NetworkResult.E_PURCHASE_NOT_ENOUGH_GEM;
                }
                break;
        }

        return result;
    }

    private NetworkResult CheckLimit(ProductDefinition product, ProductPVO pvo)
    {
        NetworkResult result = NetworkResult.SUCCESS;

        switch (product.limitType)
        {
            case ShopLimitType.None:
                break;

            case ShopLimitType.Daily:
            case ShopLimitType.Lifetime:
                if (pvo.purchaseCount >= product.purchaseLimit)
                {
                    result = NetworkResult.E_PURCHASE_LIMIT_EXCEEDED;
                }
                break;
        }

        return result;
    }

    private async Task<NetworkResult> GrantReward(RewardBase reward)
    {
        NetworkResult result = NetworkResult.SUCCESS;

        switch (reward.type)
        {
            case RewardType.Gold:
                {
                    if (reward is not CurrencyReward currency)
                    {
                        result = NetworkResult.E_PURCHASE_INVALID;
                        return result;
                    }
                    await _userService.ChangeGold(currency.amount);
                }
                break;

            case RewardType.Gem:
                {
                    if (reward is not CurrencyReward currency)
                    {
                        result = NetworkResult.E_PURCHASE_INVALID;
                        return result;
                    }

                    await _userService.ChangeGem(currency.amount);
                }

                break;

            case RewardType.Mercenary:
                {
                    if (reward is not MercenaryReward mercenary)
                    {
                        result = NetworkResult.E_PURCHASE_INVALID;
                        return result;
                    }
                }

                break;
        }

        return result;
    }
}