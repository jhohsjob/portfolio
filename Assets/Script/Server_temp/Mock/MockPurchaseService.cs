using System.Collections.Generic;
using System.Threading.Tasks;


public class MockPurchaseService
{
    private readonly ProductStorage _productStorage;

    private Dictionary<int, ProductDefinition> _products = new();

    public MockPurchaseService()
    {
        _productStorage = new();
    }

    public void SetupShopItems(IReadOnlyList<ShopItemDefinition> shopItems)
    {
        _products.Clear();

        foreach (var shopItem in shopItems)
        {
            foreach (var productItem in shopItem.productItems)
            {
                _products.Add(productItem.Id, productItem);
            }
        }

        _productStorage.SetupProduct(_products);
    }

    public async Task<List<ProductPVO>> InitializeAsync()
    {
        await _productStorage.LoadAsync();
        return new List<ProductPVO>(_productStorage.pvos);
    }

    public ProductDefinition GetProductDefinitionById(int id)
    {
        _products.TryGetValue(id, out var definition);
        return definition;
    }

    public ProductPVO GetProductPVOById(int id)
    {
        return _productStorage.GetById(id);
    }

    public async Task<bool> AddPurchaseCount(int id)
    {
        var pvo = GetProductPVOById(id);
        if (pvo == null)
        {
            return false;
        }

        pvo.purchaseCount += 1;
        pvo.totalPurchaseCount += 1;

        await _productStorage.UpdateAsync(pvo);

        return true;
    }
}