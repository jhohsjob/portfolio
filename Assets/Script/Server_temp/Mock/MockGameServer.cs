using System.Collections.Generic;
using System.Threading.Tasks;


public class MockGameServer : IGameServer
{
    private readonly MockUserService _userService;
    private readonly MockMercenaryService _mercenaryService;
    private readonly MockPurchaseService _purchaseService;

    private MockUserHandler _userHandler;
    private MockMercenaryHandler _mercenaryHandler;

    public MockGameServer(IAssetLoader assetLoader)
    {
        _userService = new MockUserService(assetLoader);
        _mercenaryService = new MockMercenaryService();
        _purchaseService = new MockPurchaseService();

        _userHandler = new MockUserHandler(_userService);
        _mercenaryHandler = new MockMercenaryHandler(_mercenaryService, _userService);
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

    public async Task<InitializeResponse> InitializeAsync(InitializeRequest request)
    {
        var user = await _userService.InitializeAsync();

        var mercenaries = await _mercenaryService.InitializeAsync();

        var products = await _purchaseService.InitializeAsync();

        return new InitializeResponse
        {
            user = user,
            mercenaries = mercenaries,
            products = products
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

    public Task<List<MercenaryPVO>> GetMercenariesAsync()
    {
        return _mercenaryService.GetMercenariesAsync();
    }

    public Task<bool> PurchaseAsync(int productId)
    {
        return _purchaseService.PurchaseAsync(productId);
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