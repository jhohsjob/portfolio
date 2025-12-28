using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public static class DataManager
{
    private static Dictionary<int, SkillData> _skillData = new();
    private static Dictionary<int, MapInfoData> _mapInfoDatas = new();

    public static async Task LoadAsync()
    {
        var tasks = new List<Task>
        {
            LoadTableAsync<MercenaryTable>("MercenaryTable", table => MercenaryHander.instance.Init(table.table)),

            LoadTableAsync<MonsterTable>("MonsterTable", table => MonsterHander.instance.Init(table.table)),

            LoadTableAsync<ProjectileTable>("ProjectileTable", table => ProjectileHander.instance.Init(table.table)),

            LoadTableAsync<DropItemTable>("DropItemTable", table => DropItemHander.instance.Init(table.table)),

            LoadTableAsync<SkillTable>("SkillTable", table => _skillData = table.table),

            LoadTableAsync<MapInfoTable>("MapInfoTable", table => _mapInfoDatas = table.table),
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

    public static List<MapInfoData> GetAllMapInfoData()
    {
        return new List<MapInfoData>(_mapInfoDatas.Values);
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