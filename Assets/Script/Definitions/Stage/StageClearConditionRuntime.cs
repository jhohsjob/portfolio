using System;


public abstract class StageClearConditionRuntime
{
    public event Action OnCleared;

    protected void RaiseCleared()
    {
        OnCleared?.Invoke();
    }

    public abstract void Init();
}