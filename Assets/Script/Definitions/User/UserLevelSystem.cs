using System.Collections.Generic;


public class UserLevelSystem
{
    private readonly List<UserLevelData> _levels = new();

    public int exp { get; private set; }
    public int level { get; private set; }

    public void Setup(List<UserLevelData> levels)
    {
        _levels.Clear();
        _levels.AddRange(levels);
    }

    public void Initialize(int exp, int level)
    {
        this.exp = exp;
        this.level = level;
    }

    public void ForceUpdate(int exp, int level)
    {
        this.exp = exp;
        this.level = level;
    }

    public void AddExp(int amount)
    {
        exp += amount;
        while (CanLevelUp())
        {
            var next = GetLevelData(level + 1);
            level = next.level;
        }
    }

    public bool CanLevelUp()
    {
        var next = GetLevelData(level + 1);

        return next != null && exp >= next.totalExp;
    }

    public UserLevelData GetLevelData(int level)
    {
        return _levels.Find(x => x.level == level);
    }
}
