using System.Collections.Generic;
using UnityEngine;


public static class GameTable
{
    private static Dictionary<int, EnemyData> _enemyDatas = new Dictionary<int, EnemyData>();
    private static Dictionary<int, ProjectileData> _projectileData = new Dictionary<int, ProjectileData>();
    private static Dictionary<int, SkillData> _skillData = new Dictionary<int, SkillData>();

    public static void Load()
    {
        {
            var table = Resources.Load<EnemyTable>("DataTable/EnemyTable");
            _enemyDatas = table.table;
        }

        {
            var table = Resources.Load<ProjectileTable>("DataTable/ProjectileTable");
            _projectileData = table.table;
        }

        {
            var table = Resources.Load<SkillTable>("DataTable/SkillTable");
            _skillData = table.table;
        }
    }

    public static EnemyData GetEnemyData(int id)
    {
        EnemyData result = null;
        if (_enemyDatas.ContainsKey(id))
        {
            result = _enemyDatas[id];
        }
        else
        {
            Debug.LogWarning("EnemyData ID : " + id + " 가 없습니다.");
        }

        return result;
    }

    public static ProjectileData GetProjectileData(int id)
    {
        ProjectileData result = null;
        if (_projectileData.ContainsKey(id))
        {
            result = _projectileData[id];
        }
        else
        {
            Debug.LogWarning("ProjectileData ID : " + id + " 가 없습니다.");
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
