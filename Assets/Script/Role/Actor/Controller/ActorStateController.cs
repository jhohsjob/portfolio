using System;
using System.Collections.Generic;
using System.Linq;


public class ActorStateController
{
    private ActorState _state = ActorState.None;
    public ActorState current => _state;

    private static readonly Dictionary<ActorState, ActorState[]> _stateTransitions = new()
    {
        { ActorState.None,   new[] { ActorState.Idle } },
        { ActorState.Idle,   new[] { ActorState.Move, ActorState.Dash, ActorState.Die } },
        { ActorState.Move,   new[] { ActorState.Idle, ActorState.Dash, ActorState.Die } },
        { ActorState.Dash,   new[] { ActorState.Idle, ActorState.Move } },
        { ActorState.Die,    Array.Empty<ActorState>() },
    };

    private Action<ActorState> _cbStateChanged;
    public event Action<ActorState> cbStateChanged
    {
        add { _cbStateChanged -= value; _cbStateChanged += value; }
        remove { _cbStateChanged -= value; }
    }

    public void SetState(ActorState next)
    {
        if (HasState(ActorState.Die))
        {
            return;
        }

        if (CanTransitionTo(next) == false)
        {
            return;
        }

        // _state |= next;
        _state = next;
        _cbStateChanged?.Invoke(_state);
    }

    private bool CanTransitionTo(ActorState next)
    {
        foreach (ActorState s in Enum.GetValues(typeof(ActorState)))
        {
            if (HasState(s) && _stateTransitions.TryGetValue(s, out var validNext))
            {
                if (validNext.Contains(next))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Clear()
    {
        _state = ActorState.None;
        _cbStateChanged = null;
    }

    public bool HasState(ActorState state) => (_state & state) == state;
}