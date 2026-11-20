using Cysharp.Threading.Tasks;
using System;
using System.Threading;


public class BattleFlow
{
    private readonly IBattleController _battleController;
    private readonly StageRunner _stageRunner;

    private readonly Stage _stage;

    public event Action<int> onStageCleared;
    public event Action<BattleStatus> onBattleEnded;

    private const float GAME_START_DELAY = 3f;

    public BattleFlow(IBattleController battleController, StageRunner stageRunner, Stage stage)
    {
        _battleController = battleController;
        _stageRunner = stageRunner;
        _stage = stage;
    }

    public void Start(CancellationToken ct)
    {
        StartAsync(ct).Forget();
    }

    private async UniTaskVoid StartAsync(CancellationToken ct)
    {
        _battleController.SetStatus(BattleStatus.Ready);

        bool canceled = await UniTask.Delay(TimeSpan.FromSeconds(GAME_START_DELAY), cancellationToken: ct).SuppressCancellationThrow();
        if (canceled)
        {
            return;
        }

        _stageRunner.Run(_stage, HandleStageCleared);
        _battleController.SetStatus(BattleStatus.Running);
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
        _battleController.SetStatus(status);

        onBattleEnded?.Invoke(status);
    }
}