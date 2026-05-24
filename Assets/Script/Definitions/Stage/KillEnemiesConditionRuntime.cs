public class KillEnemiesConditionRuntime : StageClearConditionRuntime
{
    private int _targetKillCount;
    private int _currentKillCount;

    public KillEnemiesConditionRuntime(int targetKillCount)
    {
        _targetKillCount = targetKillCount;
    }

    public override void Init()
    {
        _currentKillCount = 0;

        EventHelper.AddEventListener(EventName.EnemyDieEnd, OnEnemyDie);
        EventHelper.AddEventListener(EventName.DebugStageClear, OnDebugClear);
    }

    protected override void RaiseCleared()
    {
        EventHelper.RemoveEventListener(EventName.EnemyDieEnd, OnEnemyDie);
        EventHelper.RemoveEventListener(EventName.DebugStageClear, OnDebugClear);

        base.RaiseCleared();
    }

    private void OnEnemyDie(object sender, object param)
    {
        _currentKillCount++;

        if (_currentKillCount >= _targetKillCount)
        {
            RaiseCleared();
        }
    }

    private void OnDebugClear(object sender, object param)
    {
        RaiseCleared();
    }
}