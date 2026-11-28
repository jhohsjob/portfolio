using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;


public enum LocalizationTable
{
    None,
    Common,
    MercenaryTable,
    MonsterTable,
    ProjectileTable,
    ShopTable,
    DropItemTable,
    SkillTable,
    StatTable,
    ErrorCode
}

public class LocalizationHelper
{
    private readonly struct LocalizationCacheKey : IEquatable<LocalizationCacheKey>
    {
        public readonly LocalizationTable table;
        public readonly string key;

        public LocalizationCacheKey(LocalizationTable table, string key)
        {
            this.table = table;
            this.key = key;
        }

        public bool Equals(LocalizationCacheKey other)
        {
            return table == other.table && key == other.key;
        }

        public override bool Equals(object obj)
        {
            return obj is LocalizationCacheKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(table, key);
        }
    }

    private static readonly Dictionary<LocalizationCacheKey, string> _cache = new();

    public static string GetLocalizedString(LocalizationTable table, string key, object[] args = null)
    {
        string tableName = GetTableName(table);

        // Format 인자가 존재하는 문자열은 캐싱하지 않는다.
        if (args != null && args.Length > 0)
        {
            return LocalizationSettings.StringDatabase.GetLocalizedString(tableName, key, args);
        }

        var cacheKey = new LocalizationCacheKey(table, key);

        if (_cache.TryGetValue(cacheKey, out string value))
        {
            return value;
        }

        value = LocalizationSettings.StringDatabase.GetLocalizedString(tableName, key);

        _cache[cacheKey] = value;

        return value;
    }

    private static string GetTableName(LocalizationTable table)
    {
        return table switch
        {
            LocalizationTable.Common => "CommonTable",
            LocalizationTable.MercenaryTable => "MercenaryTable",
            LocalizationTable.MonsterTable => "MonsterTable",
            LocalizationTable.ProjectileTable => "ProjectileTable",
            LocalizationTable.ShopTable => "ShopTable",
            LocalizationTable.DropItemTable => "DropItemTable",
            LocalizationTable.SkillTable => "SkillTable",
            LocalizationTable.StatTable => "StatTable",
            LocalizationTable.ErrorCode => "ErrorCodeTable",
            _ => throw new ArgumentOutOfRangeException(nameof(table), table, null)
        };
    }

    public static void ClearCache()
    {
        _cache.Clear();
    }

    public static string GetCommonString(string key, object[] args = null)
    {
        return GetLocalizedString(LocalizationTable.Common, key, args);
    }

    public static string GetMercenaryString(string key, object[] args = null)
    {
        return GetLocalizedString(LocalizationTable.MercenaryTable, key, args);
    }

    public static string GetMonsterString(string key, object[] args = null)
    {
        return GetLocalizedString(LocalizationTable.MonsterTable, key, args);
    }

    public static string GetProjectileString(string key, object[] args = null)
    {
        return GetLocalizedString(LocalizationTable.ProjectileTable, key, args);
    }

    public static string GetShopString(string key, object[] args = null)
    {
        return GetLocalizedString(LocalizationTable.ShopTable, key, args);
    }

    public static string GetDropItemString(string key, object[] args = null)
    {
        return GetLocalizedString(LocalizationTable.DropItemTable, key, args);
    }

    public static string GetSkillString(string key, object[] args = null)
    {
        return GetLocalizedString(LocalizationTable.SkillTable, key, args);
    }

    public static string GetStatString(string key, object[] args = null)
    {
        return GetLocalizedString(LocalizationTable.StatTable, key, args);
    }

    public static string GetErrorCodeString(string key)
    {
        return GetLocalizedString(LocalizationTable.ErrorCode, key);
    }
}