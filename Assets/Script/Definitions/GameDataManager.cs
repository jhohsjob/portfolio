using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public static class GameDataManager
{
    private static Dictionary<int, SkillData> _skillData = new();

    public static async Task LoadAsync()
    {
        var tasks = new List<Task>
        {
            LoadTableAsync<MercenaryTable>("MercenaryTable", table => MercenaryManager.instance.Init(table.table)),

            LoadTableAsync<MonsterTable>("MonsterTable", table => MonsterManager.instance.Init(table.table)),

            LoadTableAsync<ProjectileTable>("ProjectileTable", table => ProjectileManager.instance.Init(table.table)),

            LoadTableAsync<DropItemTable>("DropItemTable", table => DropItemManager.instance.Init(table.table)),

            LoadTableAsync<SkillTable>("SkillTable", table => _skillData = table.table),

            LoadTableAsync<StageDefinitionTable>("StageDefinitionTable", table => StageManager.instance.Init(table.table)),

            // LoadTableAsync<ProductDefinitionTable>("ProductDefinitionTable", table => ShopManager.instance.InitProduct(table.table)),
            
            LoadTableAsync<ShopItemDefinitionTable>("ShopItemDefinitionTable", table => ShopManager.instance.InitShopItem(table.table)),
        };

        await Task.WhenAll(tasks);
    }

    private static Task LoadTableAsync<T>(string address, Action<T> onLoaded) where T : UnityEngine.Object
    {
        var tcs = new TaskCompletionSource<bool>();

        Client.asset.LoadAsset<T>(address, task =>
        {
            try
            {
                var asset = task.GetAsset<T>();
                if (asset == null)
                {
                    throw new Exception($"Data Load Failed: {address}");
                }

                onLoaded?.Invoke(asset);
                tcs.SetResult(true);
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
        });

        return tcs.Task;
    }

    public static SkillData GetSkillData(int id)
    {
        if (_skillData.TryGetValue(id, out var result))
        {
            return result;
        }

        Debug.LogWarning($"SkillData ID : {id} 가 없습니다.");
        return null;
    }
}