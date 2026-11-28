using System;


public class Stage
{
    private StageDefinition _definition;

    public int id => _definition.Id;
    public string name => _definition.stageName;
    public MapDefinition mapDefinition => _definition.mapDefinition;
    private StageClearConditionRuntime _clearCondition;

    public event Action<int> onStageCleared;

    public Stage(StageDefinition definition)
    {
        _definition = definition;
    }

    public void Init()
    {
        _clearCondition = _definition.stageClearCondition.CreateRuntime();
        _clearCondition.OnCleared += OnStageCleared;
        _clearCondition.Init();
    }

    public void Cleanup()
    {
        if (_clearCondition == null)
        {
            return;
        }

        _clearCondition.OnCleared -= OnStageCleared;
        _clearCondition.Dispose();
        _clearCondition = null;
    }

    private void OnStageCleared()
    {
        onStageCleared?.Invoke(id);
        onStageCleared = null;
    }
}
