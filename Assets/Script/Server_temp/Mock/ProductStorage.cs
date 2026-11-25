#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;


public class ProductStorage : JsonListStorage<ProductPVO>
{
    private const string FileName = "product_save.json";

    private Dictionary<int, ProductDefinition> _productDefinitions;

    public IReadOnlyList<ProductPVO> pvos => items;

    public ProductStorage() : base(FileName) { }

    public void SetupProduct(Dictionary<int, ProductDefinition> products)
    {
        _productDefinitions = products;
    }

    protected override int GetId(ProductPVO item) => item.id;

    protected override List<ProductPVO> CreateDefault()
    {
        if (_productDefinitions == null)
        {
            throw new InvalidOperationException("SetupProduct must be called before LoadAsync");
        }

        var list = new List<ProductPVO>();
        foreach (var product in _productDefinitions.Values)
        {
            list.Add(new ProductPVO
            {
                id = product.Id,
                purchaseCount = 0,
                totalPurchaseCount = 0,
                lastPurchaseTime = 0,
                lastResetTime = 0
            });
        }

        return list;
    }

    protected override void OnLoaded(List<ProductPVO> loaded)
    {
        foreach (var data in loaded)
        {
            if (_productDefinitions != null &&
                _productDefinitions.TryGetValue(data.id, out var def) &&
                def.limitType == ShopLimitType.Daily)
            {
                ResetIfNeeded(data);
            }
        }
    }

    private void ResetIfNeeded(ProductPVO data)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if (data.lastResetTime == 0)
        {
            data.lastResetTime = now;
            return;
        }

        if (TimeUtil.IsSameDayUTC(data.lastResetTime, now) == false)
        {
            data.purchaseCount = 0;
            data.lastResetTime = now;
        }
    }

#if UNITY_EDITOR
    [MenuItem("CustomMenu/DataDelete_product")]
    public static void Delete() => StoragePaths.Delete(FileName);
#endif
}
