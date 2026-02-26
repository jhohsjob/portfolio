using System;


public class Stage
{
    private StageDefinition _definition;

    public int id => _definition.id;
    public string name => _definition.stageName;
    public MapDefinition mapDefinition => _definition.mapDefinition;
    private StageClearConditionRuntime _clearCondition;

    public event Action cbStageCleared;

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
        int nextStageId = StageManager.instance.NextStageID(id);
        Client.user.SetStage(nextStageId);

        cbStageCleared?.Invoke();
    }
}
