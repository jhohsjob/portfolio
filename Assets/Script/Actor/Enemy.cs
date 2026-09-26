using System;
using UnityEngine;


public class Enemy : Actor<Monster>
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

    public override void Enter(int id, int sortingOrder, Action<ActorBase> onDied)
    {
        base.Enter(id, sortingOrder, onDied);

        _state.SetState(ActorState.Move);
    }

    protected override void Die()
    {
        _diePos = transform.localPosition;

        base.Die();
    }

    protected override void OnBodyTriggerEnter(Body other)
    {
        if (other.actor is Player player && player.state.current != ActorState.Die)
        {
            DamageResult result = CalculateDamage(player);
            player.BeHit(result.damage);

            ResetCollider();
        }
    }

    private DamageResult CalculateDamage(ActorBase target)
    {
        DamageRequest request = new DamageRequest(stat, target.stat, 1f);

        return DamageCalculator.Calculate(request);
    }
}
