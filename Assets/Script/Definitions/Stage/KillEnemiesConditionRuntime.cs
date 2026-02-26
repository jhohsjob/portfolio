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

    private void OnEnemyDie(object sender, object param)
    {
        _currentKillCount++;

        if (_currentKillCount >= _targetKillCount)
        {
            EventHelper.RemoveEventListener(EventName.EnemyDieEnd, OnEnemyDie);
            RaiseCleared();
        }
    }

    private void OnDebugClear(object sender, object param)
    {
        EventHelper.RemoveEventListener(EventName.DebugStageClear, OnDebugClear);
        RaiseCleared();
    }
}