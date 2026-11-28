using UnityEngine;


public class GroundZoneMove : IMoveBehaviour
{
    private ActorBase _actor;
    private float _timer;
    private float _damageTimer;
    private float _damageDelay = 0.5f;

    public void Init(ActorBase actor)
    {
        _actor = actor;
        _timer = 0f;
        _damageTimer = 0f;

        //var pos = _actor.transform.position;
        //_actor.transform.position = pos;
    }

    public void UpdateMove()
    {
        _timer += Time.deltaTime;

        if (_timer >= _actor.distance)
        {
            _actor.state.SetState(ActorState.Die);
        }

        _damageTimer += Time.deltaTime;
        if (_damageTimer >= _damageDelay)
        {
            _actor.ResetCollider();
            _damageTimer = 0f;
        }
    }

    public void Clear()
    {
        _timer = 0f;
        _damageTimer = 0f;
    }
}
