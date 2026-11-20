using System;


public class StageRunner
{
    private Stage _currentStage;
    private Action<int> _onCleared;

    public void Run(Stage stage, Action<int> onCleared)
    {
        Clear();

        _currentStage = stage;
        _onCleared = onCleared;

        _currentStage.onStageCleared += HandleStageCleared;
        _currentStage.Init();
    }

    public void Clear()
    {
        if (_currentStage == null)
        {
            return;
        }

        _currentStage.onStageCleared -= HandleStageCleared;

        _currentStage = null;
        _onCleared = null;
    }

    private void HandleStageCleared(int stageId)
    {
        _onCleared?.Invoke(stageId);
    }
}