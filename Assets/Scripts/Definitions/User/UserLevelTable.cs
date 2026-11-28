using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UserLevelTable", menuName = "User/UserLevelTable")]
public class UserLevelTable : ScriptableObject
{
    public List<UserLevelData> userLevels = new();

    public void GenerateLevels(int startLevel, int endLevel)
    {
        for (int level = startLevel; level <= endLevel; level++)
        {
            if (userLevels.Exists(x => x.level == level))
            {
                continue;
            }

            userLevels.Add(new UserLevelData
            {
                level = level,
                totalExp = CalculateTotalExp(level)
            });
        }

        userLevels.Sort((a, b) => a.level.CompareTo(b.level));
    }

    private int CalculateTotalExp(int level)
    {
        // todo
        return (level - 1) * 100;
    }
}

[System.Serializable]
public class UserLevelData
{
    public int level;
    public int totalExp;
}