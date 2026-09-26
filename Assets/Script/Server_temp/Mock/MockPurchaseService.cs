using System.Collections.Generic;
using System.Threading.Tasks;


public class MockPurchaseService
{
    private readonly ProductStorage _productStorage;

    public MockPurchaseService()
    {
        _productStorage = new ProductStorage();
    }

    public async Task<List<ProductPVO>> InitializeAsync()
    {
        await _productStorage.LoadAsync();
        return new List<ProductPVO>(_productStorage.pvos);
    }

    public Task<bool> PurchaseAsync(int productId)
    {
        // 1. 상품 확인
        // 2. 구매 가능 여부 확인
        // 3. 재화 확인
        // 4. 재화 차감
        // 5. 보상 지급
        // 6. 구매 횟수 증가

        return Task.FromResult(true);
    }
}