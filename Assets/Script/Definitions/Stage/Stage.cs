using System;


public class Stage
{
    private StageDefinition _definition;

    public int id => _definition.id;
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
        _clearCondition.Init();

        _clearCondition.OnCleared += OnStageCleared;
    }

    private void OnStageCleared()
    {
        onStageCleared?.Invoke(id);
        onStageCleared = null;
    }
}
