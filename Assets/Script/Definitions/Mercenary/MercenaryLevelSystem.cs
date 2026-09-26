using System;
using System.Collections.Generic;


public class MercenaryLevelSystem
{
    private readonly List<MercenaryLevelData> _levels = new();

    public void Setup(List<MercenaryLevelData> levels)
    {
        _levels.Clear();
        _levels.AddRange(levels);
    }

    public int GetAvailableMaxLevel(int level, int userLevel, int gold)
    {
        int maxAvailableLevel = Math.Min(userLevel, GameConfig.MercenaryLevelLimit);

        int affordableLevel = level;

        while (affordableLevel < maxAvailableLevel)
        {
            int cost = _levels[affordableLevel].levelUpCost;

            if (cost > gold)
            {
                break;
            }

            gold -= cost;
            affordableLevel++;
        }

        return affordableLevel;
    }

    public int GetLevelUpCost(int level, int targetLevel)
    {
        int count = targetLevel - level;
        if (count <= 0)
        {
            return -1;
        }

        int amount = 0;

        for  (int i = 0; i < count; i++)
        {
            amount += _levels[level + i].levelUpCost;
        }

        return amount;
    }

    public MercenaryLevelUpResult CheckLevelUp(int level, int targetLevel, int userLevel, int gold)
    {
        var availableMaxLevel = GetAvailableMaxLevel(level, userLevel, gold);
        if (level >= availableMaxLevel)
        {
            return MercenaryLevelUpResult.MaxLevel;
        }

        if (targetLevel > availableMaxLevel)
        {
            return MercenaryLevelUpResult.InvalidLevel;
        }

        if (GetLevelUpCost(level, targetLevel) > gold)
        {
            return MercenaryLevelUpResult.NotEnoughGold;
        }

        return MercenaryLevelUpResult.Success;
    }
}
