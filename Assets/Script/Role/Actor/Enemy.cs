using System;
using UnityEngine;


public class Enemy : Actor<Monster, MonsterDefinition>
{
    private Vector3 _diePos = Vector3.zero;
    public Vector3 diePos => _diePos;

    private float _damage = 1f;

    protected override void Awake()
    {
        base.Awake();

        team = Team.Enemy;
    }

    protected override void Update()
    {
        if (DebugBattleInput.debugEnemyPause == true)
        {
            return;
        }

        base.Update();
    }

    //public override void Init(Monster role)
    //{
    //    base.Init(role);

    //    Bind();
    //}

    public override void Enter(int id, int sortingOrder, Action<ActorBase> onDied)
    {
        base.Enter(id, sortingOrder, onDied);

        _state.SetState(ActorState.Move);
    }

    protected override void Die()
    {
        DieAfter();

        _diePos = transform.localPosition;

        base.Die();
    }

    protected override void OnBodyTriggerEnter(Body other)
    {
        if (other.TryGetComponent(out Body body))
        {
            if (body.actor is Player actor && actor.state.current != ActorState.Die)
            {
                actor.BeHit(_damage);
                ResetCollider();
            }
        }
    }
}
