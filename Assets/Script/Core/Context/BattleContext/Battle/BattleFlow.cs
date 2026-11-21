using Cysharp.Threading.Tasks;
using System;
using System.Threading;


public class BattleFlow
{
    private readonly IBattleState _battleState;
    private readonly StageRunner _stageRunner;

    private readonly Stage _stage;

    public event Action<int> onStageCleared;
    public event Action<BattleStatus> onBattleEnded;

    private const float GAME_START_DELAY = 3f;

    public BattleFlow(IBattleState battleState, StageRunner stageRunner, Stage stage)
    {
        _battleState = battleState;
        _stageRunner = stageRunner;
        _stage = stage;
    }

    public void Start(CancellationToken ct)
    {
        StartAsync(ct).Forget();
    }

    private async UniTaskVoid StartAsync(CancellationToken ct)
    {
        _battleState.SetStatus(BattleStatus.Ready);

        bool canceled = await UniTask.Delay(TimeSpan.FromSeconds(GAME_START_DELAY), cancellationToken: ct).SuppressCancellationThrow();
        if (canceled)
        {
            return;
        }

        _stageRunner.Run(_stage, HandleStageCleared);
        _battleState.SetStatus(BattleStatus.Running);
    }

    public void Win()
    {
        End(BattleStatus.Win);
    }

    public void Lose()
    {
        End(BattleStatus.Lose);
    }

    private void HandleStageCleared(int stageId)
    {
        onStageCleared?.Invoke(stageId);

        Win();
    }

    private void End(BattleStatus status)
    {
        _battleState.SetStatus(status);

        onBattleEnded?.Invoke(status);
    }
}