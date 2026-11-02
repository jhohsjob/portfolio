using System.Collections.Generic;
using System.Threading.Tasks;


public class PurchaseServiceDependencies
{
    public IGameServer gameServer;
    public User user;
    public ICurrencyService currencyService;
}

public class PurchaseService
{
    private List<ShopItemDefinition> _shopItemList = new();
    public IReadOnlyList<ShopItemDefinition> shopItemList => _shopItemList;

    private Dictionary<int, ProductDefinition> _products = new();
    private List<ProductPVO> _pvos = new();

    private readonly IGameServer _gameServer;
    private User _user;
    private readonly ICurrencyService _currencyService;

    public PurchaseService(PurchaseServiceDependencies dependencies)
    {
        _gameServer = dependencies.gameServer;
        _user = dependencies.user;
        _currencyService = dependencies.currencyService;
    }

    public void SetupShopItems(IReadOnlyList<ShopItemDefinition> shopItemDefinitions)
    {
        _shopItemList.AddRange(shopItemDefinitions);

        foreach (var shopItem in shopItemDefinitions)
        {
            foreach (var productItem in shopItem.productItems)
            {
                _products.Add(productItem.id, productItem);
            }
        }
    }

    public void InitializeServer(List<ProductPVO> pvos)
    {
        _pvos.AddRange(pvos);
    }

    public ProductPVO GetProductPVOById(int id)
    {
        return _pvos.Find(x => x.id == id);
    }

    public async Task<PurchaseResult> Purchase(int id)
    {
        if (_products.TryGetValue(id, out var product) == false)
        {
            return PurchaseResult.InvalidId;
        }

        var pvo = _pvos.Find(x => x.id == product.id);
        if (pvo == null)
        {
            return PurchaseResult.Invalid;
        }
        
        switch (product.purchaseType)
        {
            case PurchaseType.Free:
                break;

            case PurchaseType.Currency:
                if (product.currencyType != CurrencyType.None && CheckCurrency(product) == false)
                {
                    switch (product.currencyType)
                    {
                        case CurrencyType.Gold:
                            return PurchaseResult.NotEnoughGold;

                        case CurrencyType.Gem:
                            return PurchaseResult.NotEnoughGem;
                    }
                }
                break;

            case PurchaseType.IAP:
                // todo
                break;
        }

        var checkLimit = CheckLimit(product, pvo);
        if (checkLimit != PurchaseResult.Success)
        {
            return checkLimit;
        }

        PurchaseRequest request = new PurchaseRequest
        {
            productId = id
        };

        PurchaseResponse response = await _gameServer.PurchaseAsync(request);

        if (response.result == NetworkResult.SUCCESS)
        {
            pvo = response.product;

            _user.ApplyPVO(response.user);

            return PurchaseResult.Success;
        }

        return response.result switch
        {
            NetworkResult.E_PURCHASE_INVALID => PurchaseResult.Invalid,
            NetworkResult.E_PURCHASE_INVALID_ID => PurchaseResult.InvalidId,
            NetworkResult.E_PURCHASE_NOT_ENOUGH_GOLD => PurchaseResult.NotEnoughGold,
            NetworkResult.E_PURCHASE_NOT_ENOUGH_GEM => PurchaseResult.NotEnoughGem,
            NetworkResult.E_PURCHASE_LIMIT_EXCEEDED => PurchaseResult.PurchaseLimitExceeded,
            _ => PurchaseResult.Invalid
        };
    }

    private bool CheckCurrency(ProductDefinition product)
    {
        int current = _currencyService.Get(product.currencyType);
        return current >= product.price;
    }

    private PurchaseResult CheckLimit(ProductDefinition product, ProductPVO pvo)
    {
        PurchaseResult result = PurchaseResult.Success;

        switch (product.limitType)
        {
            case ShopLimitType.None:
                break;

            case ShopLimitType.Daily:
            case ShopLimitType.Lifetime:
                if (pvo.purchaseCount >= product.purchaseLimit)
                {
                    result = PurchaseResult.PurchaseLimitExceeded;
                }
                break;
        }

        return result;
    }
}