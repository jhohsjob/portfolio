using UnityEngine;


[CreateAssetMenu(menuName = "GameDefinition/KillEnemiesCondition")]
public class KillEnemiesCondition : StageClearCondition
{
    public int killCount;

    public override StageClearConditionRuntime CreateRuntime()
    {
        return new KillEnemiesConditionRuntime(killCount);
    }
}