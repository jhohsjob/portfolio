using UnityEngine;


[CreateAssetMenu(menuName = "GameDefinition/StageDefinition")]
public class StageDefinition : ScriptableObject
{
    public int id;
    public string stageName;
    public MapDefinition mapDefinition;
    public StageClearCondition stageClearCondition;
}
