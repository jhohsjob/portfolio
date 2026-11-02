using System.Collections.Generic;
using System.Threading.Tasks;


public interface IGameServer
{
    // 
    void SetupLevels(List<UserLevelData> userLevels);
    void SetupMercenaries(Dictionary<int, MercenaryDefinition> mercenaries);
    void SetupMercenaryLevels(List<MercenaryLevelData> mercenaryLevels);
    void SetupMercenaryGrades(List<MercenaryGradeData> mercenaryGrades);
    void SetupShopItems(IReadOnlyList<ShopItemDefinition> shopItems);

    // Initialize
    Task<InitializeResponse> InitializeAsync(InitializeRequest request);

    // mercenary
    Task<MercenaryAcquireResponse> MercenaryAcquireAsync(MercenaryAcquireRequest request);
    Task<MercenaryLevelUpResponse> MercenaryLevelUpAsync(MercenaryLevelUpRequest request);
    Task<MercenaryLevelResetResponse> MercenaryLevelResetAsync(MercenaryLevelResetRequest request);
    Task<MercenaryGradeUpResponse> MercenaryGradeUpAsync(MercenaryGradeUpRequest request);

    Task<List<MercenaryPVO>> GetMercenariesAsync();

    // purchase
    Task<PurchaseResponse> PurchaseAsync(PurchaseRequest request);

    // gacha
    void SetupGacha(Dictionary<int, GachaDefinition> gachas);
    Task<GachaResponse> GachaAsync(GachaRequest request);


#if DEBUG_MODE
    Task<DebugAddUserExpResponse> DebugUserAddExpAsync();
    Task<DebugAddGoldResponse> DebugAddGoldAsync();
#endif
}