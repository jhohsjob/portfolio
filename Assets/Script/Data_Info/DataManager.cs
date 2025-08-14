using System.Collections.Generic;
using UnityEngine;


public static class DataManager
{
    private static bool isLoad = false;

    private static Dictionary<int, SkillData> _skillData = new Dictionary<int, SkillData>();
    private static Dictionary<int, MapInfoData> _mapInfoDatas = new Dictionary<int, MapInfoData>();

    public static void Load()
    {
        if (isLoad == true)
        {
            return;
        }
        isLoad = true;
        Debug.Log("GameTable.Load");

        {
            var table = Resources.Load<MercenaryTable>("DataTable/MercenaryTable");
            MercenaryHander.instance.Init(table.table);
        }

        {
            var table = Resources.Load<MonsterTable>("DataTable/MonsterTable");
            MonsterHander.instance.Init(table.table);
        }

        {
            var table = Resources.Load<ProjectileTable>("DataTable/ProjectileTable");
            ProjectileHander.instance.Init(table.table);
        }

        {
            var table = Resources.Load<DropItemTable>("DataTable/DropItemTable");
            DropItemHander.instance.Init(table.table);
        }

        {
            var table = Resources.Load<SkillTable>("DataTable/SkillTable");
            _skillData = table.table;
        }

        {
            var table = Resources.Load<MapInfoTable>("DataTable/MapInfoTable");
            _mapInfoDatas = table.table;
        }
    }

    public static List<MapInfoData> GetAllMapInfoData()
    {
        var result = new List<MapInfoData>();

        foreach (var data in _mapInfoDatas.Values)
        {
            result.Add(data);
        }

        return result;
    }

    public static SkillData GetSkillData(int id)
    {
        SkillData result = null;
        if (_skillData.ContainsKey(id))
        {
            result = _skillData[id];
        }
        else
        {
            Debug.LogWarning("ProjectileData ID : " + id + " 가 없습니다.");
        }

        return result;
    }
}
