using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class GameDataLoaderContext
{
    public IAssetLoader assetLoader;
    public IGameServer gameServer;
    public User user;
    public MercenaryService mercenaryService;
    public StageService stageService;
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

            LoadTableAsync<MercenaryTable>("MercenaryTable", table =>
            {
                _context.gameServer.SetupMercenaries((table as MercenaryTable).table);
                _context.mercenaryService.SetupDefinition((table as MercenaryTable).table);
            }),

            LoadTableAsync<MonsterTable>("MonsterTable", table => MonsterManager.instance.Setup((table as MonsterTable).table)),

            LoadTableAsync<ProjectileTable>("ProjectileTable", table => ProjectileManager.instance.Setup((table as ProjectileTable).table)),

            LoadTableAsync<DropItemTable>("DropItemTable", table => DropItemManager.instance.Setup((table as DropItemTable).table)),

            LoadTableAsync<StageDefinitionTable>("StageDefinitionTable", table => _context.stageService.Init((table as StageDefinitionTable).table)),

            LoadTableAsync<ShopItemDefinitionTable>("ShopItemDefinitionTable", table => ShopManager.instance.InitShopItem((table as ShopItemDefinitionTable).table)),
        };

        await Task.WhenAll(tasks);
    }

    private Task LoadTableAsync<ScriptableObjectT>(string address, Action<ScriptableObject> onLoaded)
    {
        var tcs = new TaskCompletionSource<bool>();

        _context.assetLoader.LoadData(address, data =>
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