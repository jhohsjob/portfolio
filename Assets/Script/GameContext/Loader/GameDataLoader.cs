using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class GameDataLoaderContext
{
    public IAssetService assetService;
    public IGameServer gameServer;
    public PurchaseService purchaseService;
    public User user;
    public MercenaryService mercenaryService;
    public StageService stageService;
    public GachaService gachaService;
}

public class GameDataLoader
{
    private GameDataLoaderContext _context;

    public GameDataLoader(GameDataLoaderContext context)
    {
        _context = context;
    }

    public async Task LoadAsync()
    {
        var tasks = new List<Task>
        {
            LoadTableAsync<UserLevelTable>("UserLevelTable", table => _context.gameServer.SetupLevels((table as UserLevelTable).userLevels)),

            LoadTableAsync<MercenaryLevelTable>("MercenaryLevelTable", table =>
            {
                _context.gameServer.SetupMercenaryLevels((table as MercenaryLevelTable).mercenaryLevels);
                _context.mercenaryService.SetupLevels((table as MercenaryLevelTable).mercenaryLevels);
            }),

            LoadTableAsync<MercenaryGradeTable>("MercenaryGradeTable", table =>
            {
                _context.gameServer.SetupMercenaryGrades((table as MercenaryGradeTable).mercenaryGrades);
                _context.mercenaryService.SetupGrades((table as MercenaryGradeTable).mercenaryGrades);
            }),

            LoadTableAsync<MercenaryDefinitionTable>("MercenaryDefinitionTable", table =>
            {
                _context.gameServer.SetupMercenaries((table as MercenaryDefinitionTable).table);
                _context.mercenaryService.SetupDefinition((table as MercenaryDefinitionTable).table);
            }),

            LoadTableAsync<MonsterDefinitionTable>("MonsterDefinitionTable", table => MonsterManager.instance.Setup((table as MonsterDefinitionTable).table)),

            LoadTableAsync<ProjectileDefinitionTable>("ProjectileDefinitionTable", table => ProjectileManager.instance.Setup((table as ProjectileDefinitionTable).table)),

            LoadTableAsync<DropItemDefinitionTable>("DropItemDefinitionTable", table => DropItemManager.instance.Setup((table as DropItemDefinitionTable).table)),

            LoadTableAsync<StageDefinitionTable>("StageDefinitionTable", table => _context.stageService.Init((table as StageDefinitionTable).table)),

            LoadTableAsync<ShopItemDefinitionTable>("ShopItemDefinitionTable", table =>
            {
                _context.purchaseService.SetupShopItems((table as ShopItemDefinitionTable).table);
                _context.gameServer.SetupShopItems((table as ShopItemDefinitionTable).table);
            }),

            LoadTableAsync<GachaDefinitionTable>("GachaDefinitionTable", table =>
            {
                _context.gameServer.SetupGacha((table as GachaDefinitionTable).table);
                _context.gachaService.SetupDefinition((table as GachaDefinitionTable).table);
            }),
        };

        await Task.WhenAll(tasks);
    }

    private Task LoadTableAsync<ScriptableObjectT>(string address, Action<ScriptableObject> onLoaded)
    {
        var tcs = new TaskCompletionSource<bool>();

        _context.assetService.LoadData(address, data =>
        {
            try
            {
                if (data == null)
                {
                    throw new Exception($"Data Load Failed: {address}");
                }

                onLoaded?.Invoke(data);
                tcs.SetResult(true);
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
        });

        return tcs.Task;
    }

    //public SkillDefinition GetSkillData(int id)
    //{
    //    if (_skillData.TryGetValue(id, out var result))
    //    {
    //        return result;
    //    }

    //    Debug.LogWarning($"SkillData ID : {id} 가 없습니다.");
    //    return null;
    //}
}