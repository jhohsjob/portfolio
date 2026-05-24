using System;


public class BattleRewardRuntime
{
    public int gold { get; private set; }

    public event Action<int> onGoldChanged;

    public void AddGold(int gold)
    {
        this.gold += gold;

        onGoldChanged?.Invoke(this.gold);
    }

    public void Clear()
    {
        gold = 0;
    }
}