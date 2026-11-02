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
    ErrorCode
}

public class LocalManager
{

    public static string GetLocalizedString(LocalizationTable table, string key, object[] args = null)
    {
        string tableName = table switch
        {
            LocalizationTable.Common => "CommonTable",
            LocalizationTable.MercenaryTable => "MercenaryTable",
            LocalizationTable.MonsterTable => "MonsterTable",
            LocalizationTable.ProjectileTable => "ProjectileTable",
            LocalizationTable.ShopTable => "ShopTable",
            LocalizationTable.DropItemTable => "DropItemTable",
            LocalizationTable.SkillTable => "SkillTable",
            LocalizationTable.ErrorCode => "ErrorCodeTable",
            _ => throw new System.ArgumentOutOfRangeException(nameof(table), table, null)
        };

        return LocalizationSettings.StringDatabase.GetLocalizedString(tableName, key, args);
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

    public static string GetErrorCodeString(string key)
    {
        return GetLocalizedString(LocalizationTable.ErrorCode, key);
    }
}
