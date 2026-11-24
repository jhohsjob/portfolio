using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class MockGachaHandler
{
    private MockGachaDomain _gachaDomain;
    private MockUserDomain _userDomain;
    private MockMercenaryDomain _mercenaryDomain; 

    public MockGachaHandler(MockGachaDomain gacha, MockUserDomain user, MockMercenaryDomain mercenary)
    {
        _gachaDomain = gacha;
        _userDomain = user;
        _mercenaryDomain = mercenary;
    }

    public async Task<GachaResponse> GachaAsync(GachaRequest request)
    {
        GachaResponse response = new();

        var gacha = _gachaDomain.GetGachaDefinitionById(request.gachaId);
        if (gacha == null)
        {
            response.result = NetworkResult.E_GACHA_INVALID_ID;
            return response;
        }

        var pvo = _gachaDomain.GetGachaPVOById(gacha.Id);
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
                var checkCurrency = CheckCurrency(gacha, totalPrice);
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
                    if (reward is not MercenaryReward m || _mercenaryDomain.GetMercenaryPVOById(m.mercenaryId) == null)
                    {
                        response.result = NetworkResult.E_GACHA_INVALID;
                        return response;
                    }
                    break;
            }
        }

        bool charged = gacha.purchaseType == PurchaseType.Currency && totalPrice > 0;
        if (charged)
        {
            await ChangeCurrency(gacha.currencyType, -totalPrice);
        }

        foreach (var reward in rewards)
        {
            switch (reward.type)
            {
                case RewardType.Mercenary:
                    if (reward is MercenaryReward m)
                    {
                        bool success = await _mercenaryDomain.AddShard(m.mercenaryId, 1);
                        if (success == false)
                        {
                            if (charged)
                            {
                                await ChangeCurrency(gacha.currencyType, totalPrice);
                            }

                            response.result = NetworkResult.E_MERCENARY_INVALID;
                            return response;
                        }
                    }
                    break;
            }
        }

        await _gachaDomain.GachaSuccessAsync(gacha.Id, request.drawCount);

        response.gachaPVO = _gachaDomain.GetGachaPVOById(gacha.Id);
        response.userPVO = _userDomain.GetUser();

        response.mercenaryPVOs = new();
        foreach (var reward in rewards)
        {
            switch (reward.type)
            {
                case RewardType.Mercenary:
                    if (reward is MercenaryReward m)
                    {
                        response.mercenaryPVOs[m.mercenaryId] = _mercenaryDomain.GetMercenaryPVOById(m.mercenaryId);
                    }
                    break;
            }
        }
        response.rewards = rewards;
        response.result = NetworkResult.SUCCESS;

        return response;
    }

    private NetworkResult CheckCurrency(GachaDefinition gacha, int totalPrice)
    {
        switch (gacha.currencyType)
        {
            case CurrencyType.Gold:
                return _userDomain.gold < totalPrice ? NetworkResult.E_GACHA_NOT_ENOUGH_GOLD : NetworkResult.SUCCESS;

            case CurrencyType.Gem:
                return _userDomain.gem < totalPrice ? NetworkResult.E_GACHA_NOT_ENOUGH_GEM : NetworkResult.SUCCESS;

            default:
                return NetworkResult.E_GACHA_INVALID;
        }
    }

    private Task<bool> ChangeCurrency(CurrencyType type, int amount)
    {
        return type switch
        {
            CurrencyType.Gold => _userDomain.ChangeGold(amount),
            CurrencyType.Gem => _userDomain.ChangeGem(amount),
            _ => Task.FromResult(false)
        };
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