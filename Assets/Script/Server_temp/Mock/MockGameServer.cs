using System.Collections.Generic;
using System.Threading.Tasks;


public class MockGameServer : IGameServer
{
    private readonly MockUserService _userService;
    private readonly MockMercenaryService _mercenaryService;
    private readonly MockPurchaseService _purchaseService;
    private readonly MockGachaService _gachaService;

    private MockUserHandler _userHandler;
    private MockMercenaryHandler _mercenaryHandler;
    private MockPurchaseHandler _purchaseHandler;
    private MockGachaHandler _gachaHandler;

    public MockGameServer(IAssetService assetService)
    {
        _userService = new MockUserService(assetService);
        _mercenaryService = new MockMercenaryService();
        _purchaseService = new MockPurchaseService();
        _gachaService = new MockGachaService();

        _userHandler = new MockUserHandler(_userService);
        _mercenaryHandler = new MockMercenaryHandler(_mercenaryService, _userService);
        _purchaseHandler = new MockPurchaseHandler(_purchaseService, _userService);
        _gachaHandler = new MockGachaHandler(_gachaService, _userService, _mercenaryService);
    }

    public void SetupLevels(List<UserLevelData> userLevels)
    {
        _userService.SetupLevels(userLevels);
    }

    public void SetupMercenaries(Dictionary<int, MercenaryDefinition> mercenaries)
    {
        _mercenaryService.SetupMercenaries(mercenaries);
    }

    public void SetupMercenaryLevels(List<MercenaryLevelData> mercenaryLevels)
    {
        _mercenaryService.SetupMercenaryLevels(mercenaryLevels);
    }

    public void SetupMercenaryGrades(List<MercenaryGradeData> mercenaryGrades)
    {
        _mercenaryService.SetupMercenaryGrades(mercenaryGrades);
    }

    public void SetupShopItems(IReadOnlyList<ShopItemDefinition> shopItems)
    {
        _purchaseService.SetupShopItems(shopItems);
    }

    public void SetupGacha(Dictionary<int, GachaDefinition> gachas)
    {
        _gachaService.SetupGacha(gachas);
    }

    public async Task<InitializeResponse> InitializeAsync(InitializeRequest request)
    {
        var user = await _userService.InitializeAsync();

        var mercenaries = await _mercenaryService.InitializeAsync();

        var products = await _purchaseService.InitializeAsync();

        var gachas = await _gachaService.InitializeAsync();

        return new InitializeResponse
        {
            user = user,
            mercenaries = mercenaries,
            products = products,
            gachas = gachas
        };
    }

    public Task<MercenaryAcquireResponse> MercenaryAcquireAsync(MercenaryAcquireRequest request)
    {
        return _mercenaryHandler.MercenaryAcquireAsync(request);
    }

    public Task<MercenaryLevelUpResponse> MercenaryLevelUpAsync(MercenaryLevelUpRequest request)
    {
        return _mercenaryHandler.MercenaryLevelUpAsync(request);
    }

    public Task<MercenaryLevelResetResponse> MercenaryLevelResetAsync(MercenaryLevelResetRequest request)
    {
        return _mercenaryHandler.MercenaryLevelResetAsync(request);
    }

    public Task<MercenaryGradeUpResponse> MercenaryGradeUpAsync(MercenaryGradeUpRequest request)
    {
        return _mercenaryHandler.MercenaryGradeUpAsync(request);
    }

    public Task<List<MercenaryPVO>> GetMercenariesAsync()
    {
        return _mercenaryService.GetMercenariesAsync();
    }

    public Task<PurchaseResponse> PurchaseAsync(PurchaseRequest request)
    {
        return _purchaseHandler.PurchaseAsync(request);
    }

    public Task<GachaResponse> GachaAsync(GachaRequest request)
    {
        return _gachaHandler.GachaAsync(request);
    }

#if DEBUG_MODE
    public Task<DebugAddUserExpResponse> DebugUserAddExpAsync()
    {
        return _userHandler.DebugUserAddExpAsync();
    }

    public Task<DebugAddGoldResponse> DebugAddGoldAsync()
    {
        return _userHandler.DebugAddGoldAsync();
    }
#endif
}