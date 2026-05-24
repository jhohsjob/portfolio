using System;


public abstract class StageClearConditionRuntime
{
    public event Action OnCleared;

    protected virtual void RaiseCleared()
    {
        OnCleared?.Invoke();
    }

    public abstract void Init();
}