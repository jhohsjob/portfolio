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
        if (targetGrade <= grade || targetGrade > GameConfig.MercenaryGradeLimit)
        {
            return -1;
        }

        int amount = 0;

        for  (int i = 0; i < targetGrade; i++)
        {
            amount += _grades[i].gradeUpCost;
        }

        return amount;
    }

    // 표시용: 현재 등급 → 다음 등급 구간의 (진행도, 필요량)
    public (int current, int required) GetGradeUpProgress(int grade, int shardCount)
    {
        if (grade >= GameConfig.MercenaryGradeLimit)
        {
            return (0, 0);
        }

        int previous = 0;
        for (int i = 0; i < grade; i++)
        {
            previous += _grades[i].gradeUpCost;
        }

        int required = _grades[grade].gradeUpCost;
        int current = Mathf.Max(0, shardCount - previous);

        return (current, required);
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
