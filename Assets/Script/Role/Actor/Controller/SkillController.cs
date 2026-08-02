using System.Collections.Generic;


public class SkillController
{
    private List<Skill> _skills = new();

    private Skill _currentSkill;
    private int _skillLevel;

    public void Init(List<Skill> skills)
    {
        _skillLevel = 1;

        _skills.Clear();
        _skills.AddRange(skills);

        UseSkill();
    }

    private void SetSkillLevel(int level)
    {
        if (level < 1 || level > _skills.Count)
        {
            return;
        }

        if (_skillLevel == level)
        {
            return;
        }

        _skillLevel = level;
    }

    public void UseSkill()
    {
        if (_skills.Count == 0)
        {
            return;
        }

        if (_skillLevel < 1 || _skillLevel > _skills.Count)
        {
            return;
        }

        _currentSkill?.SetUsed(false);

        _currentSkill = _skills[_skillLevel - 1];
        _currentSkill.SetUsed(true);
    }

    public void OnElementLevelUp()
    {
        SetSkillLevel(_skillLevel + 1);
        UseSkill();
    }
}