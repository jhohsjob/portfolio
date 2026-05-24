using System;
using System.Collections;
using UnityEngine;


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

    public void Start()
    {
        IEnumeratorTool.instance.StartCoroutine(CoStart());
    }

    private IEnumerator CoStart()
    {
        _battleController.SetStatus(BattleStatus.Ready);

        yield return new WaitForSeconds(GAME_START_DELAY);

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