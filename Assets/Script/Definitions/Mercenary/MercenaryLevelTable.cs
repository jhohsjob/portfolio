using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MercenaryLevelTable", menuName = "Mercenary/MercenaryLevelTable")]
public class MercenaryLevelTable : ScriptableObject
{
    public List<MercenaryLevelData> mercenaryLevels = new();

    public void GenerateLevels(int startLevel, int endLevel)
    {
        for (int level = startLevel; level <= endLevel; level++)
        {
            if (mercenaryLevels.Exists(x => x.level == level))
            {
                continue;
            }

            mercenaryLevels.Add(new MercenaryLevelData
            {
                level = level,
                levelUpCost = CalculateLevelUpGold(level)
            });
        }

        mercenaryLevels.Sort((a, b) => a.level.CompareTo(b.level));
    }

    private int CalculateLevelUpGold(int level)
    {
        // todo
        return (level - 1) * 100;
    }
}

[System.Serializable]
public class MercenaryLevelData
{
    public int level;
    public int levelUpCost;
}