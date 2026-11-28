using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MercenaryGradeTable", menuName = "Mercenary/MercenaryGradeTable")]
public class MercenaryGradeTable : ScriptableObject
{
    public List<MercenaryGradeData> mercenaryGrades = new();

    public void GenerateGrades(int startGrade, int endGrade)
    {
        for (int grade = startGrade; grade <= endGrade; grade++)
        {
            if (mercenaryGrades.Exists(x => x.grade == grade))
            {
                continue;
            }

            mercenaryGrades.Add(new MercenaryGradeData
            {
                grade = grade,
                gradeUpCost = CalculateGradeUpGold(grade),
            });
        }

        mercenaryGrades.Sort((a, b) => a.grade.CompareTo(b.grade));
    }

    private int CalculateGradeUpGold(int grade)
    {
        return grade * 10;
    }
}

[System.Serializable]
public class MercenaryGradeData
{
    public int grade;
    public int gradeUpCost;
}