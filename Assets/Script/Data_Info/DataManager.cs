using System.Collections.Generic;
using UnityEngine;


public static class DataManager
{
    private static Dictionary<int, SkillData> _skillData = new Dictionary<int, SkillData>();
    private static Dictionary<int, MapInfoData> _mapInfoDatas = new Dictionary<int, MapInfoData>();

    public static void Load()
    {
        Debug.Log("GameTable.Load");

        Client.asset.LoadAsset<MercenaryTable>("MercenaryTable", (task) =>
        {
            MercenaryHander.instance.Init(task.GetAsset<MercenaryTable>().table);
        });

        Client.asset.LoadAsset<MonsterTable>("MonsterTable", (task) =>
        {
            MonsterHander.instance.Init(task.GetAsset<MonsterTable>().table);
        });

        Client.asset.LoadAsset<ProjectileTable>("ProjectileTable", (task) =>
        {
            ProjectileHander.instance.Init(task.GetAsset<ProjectileTable>().table);
        });

        Client.asset.LoadAsset<DropItemTable>("DropItemTable", (task) =>
        {
            DropItemHander.instance.Init(task.GetAsset<DropItemTable>().table);
        });

        Client.asset.LoadAsset<SkillTable>("SkillTable", (task) =>
        {
            _skillData = task.GetAsset<SkillTable>().table;
        });

        Client.asset.LoadAsset<MapInfoTable>("MapInfoTable", (task) =>
        {
            _mapInfoDatas = task.GetAsset<MapInfoTable>().table;
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
