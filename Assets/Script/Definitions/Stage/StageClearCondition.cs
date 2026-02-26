using UnityEngine;


public abstract class StageClearCondition : ScriptableObject
{
    public abstract StageClearConditionRuntime CreateRuntime();
}