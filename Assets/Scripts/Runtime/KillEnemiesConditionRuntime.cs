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

        Unsubscribe();
        Subscribe();
    }

    protected override void RaiseCleared()
    {
        Unsubscribe();

        base.RaiseCleared();
    }

    public override void Dispose()
    {
        Unsubscribe();

        base.Dispose();
    }

    private void Subscribe()
    {
        EventHelper.AddEventListener(EventName.EnemyDieEnd, OnEnemyDie);
        EventHelper.AddEventListener(EventName.DebugStageClear, OnDebugClear);
    }

    private void Unsubscribe()
    {
        EventHelper.RemoveEventListener(EventName.EnemyDieEnd, OnEnemyDie);
        EventHelper.RemoveEventListener(EventName.DebugStageClear, OnDebugClear);
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