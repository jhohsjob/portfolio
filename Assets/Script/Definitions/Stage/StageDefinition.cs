using UnityEngine;


[CreateAssetMenu(menuName = "GameDefinition/StageDefinition")]
public class StageDefinition : DefinitionBase
{
    public string stageName;
    public MapDefinition mapDefinition;
    public StageClearCondition stageClearCondition;
}
