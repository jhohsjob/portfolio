using UnityEngine;


public class ChaseTargetMove : IMoveBehaviour
{
    private ActorBase _actor;
    private Transform _target;

    private float _stopDistance;

    private Vector3 _moveDirection;

    public void Init(ActorBase actor)
    {
        _actor = actor;
    }

    public void Setup(Transform target, float stopDistance)
    {
        _target = target;
        _stopDistance = stopDistance;
    }

    public void UpdateMove()
    {
        if (_actor == null || _target == null)
        {
            return;
        }

        UpdateDirection();
        Move();
    }

    public void Clear()
    {
        _target = null;
    }

    private void UpdateDirection()
    {
        var dir = _target.position - _actor.transform.position;

        _moveDirection = dir.normalized;

        //_actor.body.FlipX(_moveDirection.x);
        _actor.SetFlip(_moveDirection);
    }

    private void Move()
    {
        float distance = Vector3.Distance(_actor.transform.position, _target.position);
        if (distance <= _stopDistance)
        {
            return;
        }

        _actor.transform.Translate(_moveDirection * _actor.moveSpeed * Time.deltaTime);
    }
}