using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;


public static class PurchaseFailReasonExtensions
{
    public static string ToKey(this PurchaseFailReason reason)
    {
        return reason switch
        {
            PurchaseFailReason.NotEnoughCurrency => "failed_not_enough_currency",
            PurchaseFailReason.PurchaseLimitExceeded => "failed_purchase_limit_exceeded",
            PurchaseFailReason.InvalidProduct => "failed_invalid_product",
            PurchaseFailReason.AlreadyOwned => "failed_already_owned",
            PurchaseFailReason.ServerError => "failed_server_error",
            _ => "failed_unknown_error"
        };
    }

    public static string ToMessage(this PurchaseFailReason reason)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(LocalTable.ShopTable, reason.ToKey());
    }
}

public class PurchaseService
{
    private readonly ICurrencyService _currencyService;
    private readonly ProductStorage _productStorage;

    private RewardExecutor _rewardExecutor;

    public PurchaseService(ICurrencyService currencyService, ProductStorage productStorage)
    {
        _currencyService = currencyService;
        _productStorage = productStorage;

        _rewardExecutor = new RewardExecutor(currencyService);
    }

    public async Task<(bool, PurchaseFailReason reason)> TryPurchase(ProductDefinition product)
    {
        if (product == null)
        {
            return (false, PurchaseFailReason.InvalidProduct);
        }

        if (CanAfford(product) == false)
        {
            return (false, PurchaseFailReason.NotEnoughCurrency);
        }

        if (CanLimit(product) == false)
        {
            return (false, PurchaseFailReason.PurchaseLimitExceeded);
        }

        var success = await GiveReward(product);
        if (success == false)
        {
            return (false, PurchaseFailReason.ServerError);
        }

        await AddPurchaseCount(product);

        return (true, PurchaseFailReason.None);
    }

    private bool CanAfford(ProductDefinition product)
    {
        int current = _currencyService.Get(product.currencyType);
        return current >= product.price;
    }

    private bool CanLimit(ProductDefinition product)
    {
        var saveData = _productStorage.Get(product.id);

        switch (product.limitType)
        {
            case ShopLimitType.Daily:
            case ShopLimitType.Lifetime:
                return saveData.purchaseCount < product.maxPurchaseCount;

            default:
                return false;
        }
    }

    private void SpendCurrency(ProductDefinition product)
    {
        _currencyService.Change(product.currencyType, -product.price);
    }

    private async Task<bool> GiveReward(ProductDefinition product)
    {
        foreach (var reward in product.rewards)
        {
            var result = reward.CanGive();

            if (result.success == false)
            {
                Debug.LogWarning(result.message);
                return false;
            }
        }

        SpendCurrency(product);

        foreach (var reward in product.rewards)
        {
            await _rewardExecutor.Apply(reward);
        }

        return true;
    }

    private async Task AddPurchaseCount(ProductDefinition product)
    {
        var data = _productStorage.Get(product.id);
        data.purchaseCount++;
        data.lastPurchaseTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        await _productStorage.Update(data);
    }
}