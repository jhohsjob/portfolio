using System.Collections.Generic;
using System.Threading.Tasks;


public interface IGameServer
{
    // 
    void SetupLevels(List<UserLevelData> userLevels);
    void SetupMercenaries(Dictionary<int, MercenaryDefinition> mercenaries);
    void SetupMercenaryLevels(List<MercenaryLevelData> mercenaryLevels);

    // Initialize
    Task<InitializeResponse> InitializeAsync(InitializeRequest request);

    // mercenary
    Task<MercenaryAcquireResponse> MercenaryAcquireAsync(MercenaryAcquireRequest request);
    Task<MercenaryLevelUpResponse> MercenaryLevelUpAsync(MercenaryLevelUpRequest request);
    Task<MercenaryLevelResetResponse> MercenaryLevelResetAsync(MercenaryLevelResetRequest request);

    Task<List<MercenaryPVO>> GetMercenariesAsync();

    Task<bool> PurchaseAsync(int productId);


#if DEBUG_MODE
    Task<DebugAddUserExpResponse> DebugUserAddExpAsync();
    Task<DebugAddGoldResponse> DebugAddGoldAsync();
#endif
}