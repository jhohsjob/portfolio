using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class GameDataLoaderContext
{
    public IAssetLoader assetLoader;
    public StageService stageService;
}

public class GameDataLoader
{
    private GameDataLoaderContext _context;

    private static Dictionary<int, SkillData> _skillData = new();

    public GameDataLoader(GameDataLoaderContext context)
    {
        _context = context;
    }

    public async Task LoadAsync()
    {
        var tasks = new List<Task>
        {
            LoadTableAsync<MercenaryTable>("MercenaryTable", table => MercenaryManager.instance.Setup((table as MercenaryTable).table)),

            LoadTableAsync<MonsterTable>("MonsterTable", table => MonsterManager.instance.Setup((table as MonsterTable).table)),

            LoadTableAsync<ProjectileTable>("ProjectileTable", table => ProjectileManager.instance.Setup((table as ProjectileTable).table)),

            LoadTableAsync<DropItemTable>("DropItemTable", table => DropItemManager.instance.Setup((table as DropItemTable).table)),

            LoadTableAsync<SkillTable>("SkillTable", table => _skillData = (table as SkillTable).table),

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

    public SkillData GetSkillData(int id)
    {
        if (_skillData.TryGetValue(id, out var result))
        {
            return result;
        }

        Debug.LogWarning($"SkillData ID : {id} 가 없습니다.");
        return null;
    }
}