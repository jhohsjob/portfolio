using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class MockGachaHandler
{
    private MockGachaService _gachaService;
    private MockUserService _userService;
    private MockMercenaryService _mercenaryService; 

    public MockGachaHandler(MockGachaService gacha, MockUserService user, MockMercenaryService mercenary)
    {
        _gachaService = gacha;
        _userService = user;
        _mercenaryService = mercenary;
    }

    public async Task<GachaResponse> GachaAsync(GachaRequest request)
    {
        GachaResponse response = new();

        var gacha = _gachaService.GetGachaDefinitionById(request.gachaId);
        if (gacha == null)
        {
            response.result = NetworkResult.E_GACHA_INVALID_ID;
            return response;
        }

        var pvo = _gachaService.GetGachaPVOById(gacha.id);
        if (pvo == null)
        {
            response.result = NetworkResult.E_GACHA_INVALID;
            return response;
        }

        if (request.drawCount <= 0 || gacha.drawCount == null || gacha.drawCount.Contains(request.drawCount) == false)
        {
            response.result = NetworkResult.E_GACHA_INVALID_DRAW_COUNT;
            return response;
        }

        if (gacha.rewards == null || gacha.rewards.Count == 0 ||
            gacha.rewards.Any(x => x.reward == null || float.IsNaN(x.weight) || float.IsInfinity(x.weight) || x.weight < 0f))
        {
            response.result = NetworkResult.E_GACHA_INVALID;
            return response;
        }
        
        float totalWeight = gacha.rewards.Sum(x => x.weight);
        if (float.IsNaN(totalWeight) || float.IsInfinity(totalWeight) || totalWeight <= 0f)
        {
            response.result = NetworkResult.E_GACHA_INVALID;
            return response;
        }

        if (gacha.price < 0)
        {
            response.result = NetworkResult.E_GACHA_INVALID;
            return response;
        }

        int totalPrice;
        try
        {
            totalPrice = checked(gacha.price * request.drawCount);
        }
        catch (OverflowException)
        {
            response.result = NetworkResult.E_GACHA_INVALID;
            return response;
        }
        
        switch (gacha.purchaseType)
        {
            case PurchaseType.Free:
                break;

            case PurchaseType.Currency:
                var checkCurrency = CheckCurrency(gacha);
                if (checkCurrency != NetworkResult.SUCCESS)
                {
                    response.result = checkCurrency;
                    return response;
                }
                break;

            default:
                response.result = NetworkResult.E_GACHA_INVALID;
                return response;
        }

        List<RewardBase> rewards = new();

        for (int i = 0; i < request.drawCount; i++)
        {
            var selected = DrawReward(gacha.rewards, totalWeight);
            rewards.Add(selected.reward);
        }

        foreach (var reward in rewards)
        {
            switch (reward.type)
            {
                case RewardType.Mercenary:
                    if (reward is MercenaryReward mercenaryReward)
                    {
                        bool success = await _mercenaryService.AddShard(mercenaryReward.mercenaryId, 1);
                        if (success == false)
                        {
                            response.result = NetworkResult.E_MERCENARY_INVALID;
                            return response;
                        }
                    }
                    break;
            }
        }

        if (gacha.purchaseType == PurchaseType.Currency)
        {
            switch (gacha.currencyType)
            {
                case CurrencyType.Gold:
                    await _userService.ChangeGold(-totalPrice);
                    break;

                case CurrencyType.Gem:
                    await _userService.ChangeGem(-totalPrice);
                    break;
            }
        }

        response.gachaPVO = pvo;
        response.userPVO = _userService.GetUser();

        response.mercenaryPVOs = new();
        foreach (var reward in rewards)
        {
            switch (reward.type)
            {
                case RewardType.Mercenary:
                    if (reward is MercenaryReward mercenaryReward)
                    {
                        response.mercenaryPVOs[mercenaryReward.mercenaryId] = _mercenaryService.GetMercenaryPVOById(mercenaryReward.mercenaryId);
                    }
                    break;
            }
        }
        response.rewards = rewards;
        response.result = NetworkResult.SUCCESS;

        return response;
    }

    private NetworkResult CheckCurrency(GachaDefinition gacha)
    {
        NetworkResult result = NetworkResult.SUCCESS;

        switch (gacha.currencyType)
        {
            case CurrencyType.Gold:
                if (_userService.gold < gacha.price)
                {
                    result = NetworkResult.E_PURCHASE_NOT_ENOUGH_GOLD;
                }
                break;

            case CurrencyType.Gem:
                if (_userService.gem < gacha.price)
                {
                    result = NetworkResult.E_PURCHASE_NOT_ENOUGH_GEM;
                }
                break;

            default:
                result = NetworkResult.E_GACHA_INVALID;
                break;
        }

        return result;
    }

    private GachaRewardDefinition DrawReward(List<GachaRewardDefinition> rewards, float totalWeight)
    {
        float value = UnityEngine.Random.value * totalWeight;

        foreach (var reward in rewards)
        {
            value -= reward.weight;

            if (value < 0f)
            {
                return reward;
            }
        }

        // 부동소수점 오차 보정
        return rewards.Last(x => x.weight > 0f);
    }
}