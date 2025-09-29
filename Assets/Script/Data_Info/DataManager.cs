using System;
using System.Collections.Generic;
using UnityEngine;


public static class DataManager
{
    private static Dictionary<int, SkillData> _skillData = new Dictionary<int, SkillData>();
    private static Dictionary<int, MapInfoData> _mapInfoDatas = new Dictionary<int, MapInfoData>();

    public static bool IsLoaded { get; private set; }

    private static int _pendingCount = 0;
    private static Action _onComplete;

    public static void Load(Action callback)
    {
        Debug.Log("GameTable.Load");

        IsLoaded = false;
        _pendingCount = 0;
        _onComplete = callback;

        Load<MercenaryTable>("MercenaryTable",table => MercenaryHander.instance.Init(table.table));

        Load<MonsterTable>("MonsterTable",table => MonsterHander.instance.Init(table.table));

        Load<ProjectileTable>("ProjectileTable",table => ProjectileHander.instance.Init(table.table));

        Load<DropItemTable>("DropItemTable",table => DropItemHander.instance.Init(table.table));

        Load<SkillTable>("SkillTable",table => _skillData = table.table);

        Load<MapInfoTable>("MapInfoTable",table => _mapInfoDatas = table.table);
    }

    private static void Load<T>(string address, Action<T> callback) where T : UnityEngine.Object
    {
        _pendingCount++;

        Client.asset.LoadAsset<T>(address, task =>
        {
            var asset = task.GetAsset<T>();
            if (asset != null)
            {
                callback?.Invoke(asset);
            }
            else
            {
                Debug.LogError("Data Load Failed");
            }

            _pendingCount--;
            if (_pendingCount == 0)
            {
                IsLoaded = true;
                _onComplete?.Invoke();
            }
        });
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
        Debug.LogWarning("ProjectileData ID : " + id + " 가 없습니다.");

        return null;
    }
}