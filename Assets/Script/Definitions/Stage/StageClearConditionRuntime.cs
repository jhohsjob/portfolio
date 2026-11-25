using System;


public abstract class StageClearConditionRuntime : IDisposable
{
    public event Action OnCleared;

    public abstract void Init();

    protected virtual void RaiseCleared()
    {
        OnCleared?.Invoke();
    }

    public virtual void Dispose()
    {
        OnCleared = null;
    }
}