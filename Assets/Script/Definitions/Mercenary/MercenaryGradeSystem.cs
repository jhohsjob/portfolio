using System.Collections.Generic;
using UnityEngine;


public class MercenaryGradeSystem
{
    private readonly List<MercenaryGradeData> _grades = new();

    public void Setup(List<MercenaryGradeData> grades)
    {
        if (grades == null)
        {
            Debug.LogError("Mercenary grades is null");
            return;
        }

        if (GameConfig.MercenaryGradeLimit > grades.Count)
        {
            Debug.LogError("Mismatch mercenary grade count");
            return;
        }

        _grades.Clear();
        _grades.AddRange(grades);
    }

    public int GetAvailableMaxGrade()
    {
        return GameConfig.MercenaryGradeLimit;
    }

    public int GetGradeUpCost(int grade, int targetGrade)
    {
        int count = targetGrade - grade;
        if (count <= 0 || GameConfig.MercenaryGradeLimit < grade + count)
        {
            return -1;
        }

        int amount = 0;

        for  (int i = 0; i < count; i++)
        {
            amount += _grades[grade + i].gradeUpCost;
        }

        return amount;
    }

    public MercenaryGradeUpResult CheckGradeUp(int grade, int targetGrade, int shardCount)
    {
        var availableMaxGrade = GetAvailableMaxGrade();
        if (grade >= availableMaxGrade)
        {
            return MercenaryGradeUpResult.MaxGrade;
        }

        if (targetGrade > availableMaxGrade)
        {
            return MercenaryGradeUpResult.InvalidGrade;
        }

        if (GetGradeUpCost(grade, targetGrade) > shardCount)
        {
            return MercenaryGradeUpResult.NotEnoughShard;
        }

        return MercenaryGradeUpResult.Success;
    }

    public NetworkResult CheckGradeUpServer(int grade, int targetGrade, int shardCount)
    {
        var availableMaxGrade = GetAvailableMaxGrade();
        if (grade >= availableMaxGrade)
        {
            return NetworkResult.E_MERCENARY_MAX_GRADE;
        }

        if (targetGrade > availableMaxGrade)
        {
            return NetworkResult.E_MERCENARY_INVALID_GRADE;
        }

        if (GetGradeUpCost(grade, targetGrade) > shardCount)
        {
            return NetworkResult.E_MERCENARY_NOT_ENOUGH_SHARD;
        }

        return NetworkResult.SUCCESS;
    }
}
