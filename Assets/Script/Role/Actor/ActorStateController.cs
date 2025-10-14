using System;
using System.Collections.Generic;
using System.Linq;


public class ActorStateController
{
    private ActorState _state = ActorState.None;

    private static readonly Dictionary<ActorState, ActorState[]> _stateTransitions = new()
    {
        { ActorState.Dash,   new[] { ActorState.Idle, ActorState.Move } },
        { ActorState.Die,    new ActorState[0] },
    };

    public ActorState Current => _state;

    public event Action<ActorState> OnStateChanged;

    public void SetState(ActorState next)
    {
        if (HasState(ActorState.Die))
        {
            return;
        }
        
        bool canTransition = true;

        foreach (ActorState s in Enum.GetValues(typeof(ActorState)))
        {
            if (HasState(s) == false || s == ActorState.None)
            {
                continue;
            }

            if (_stateTransitions.TryGetValue(s, out var validNext))
            {
                if (validNext.Contains(next) == false)
                {
                    canTransition = false;
                    break;
                }
            }
        }

        if (canTransition == false)
        {
            return;
        }

        _state = next;

        OnStateChanged?.Invoke(_state);
    }

    public void Clear() => _state = ActorState.None;
    public bool HasState(ActorState state) => (_state & state) == state;
}