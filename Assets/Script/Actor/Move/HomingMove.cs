using UnityEngine;


public class HomingMove : IMoveBehaviour
{
    private ActorBase _actor;
    private Transform _target;
    private Vector3 _prevPos;
    private float _moveDistance;

    public void Init(ActorBase actor)
    {
        _actor = actor;
        _prevPos = Vector3.zero;
        _moveDistance = 0;
    }

    public void Setup(Transform target)
    {
        _target = target;
    }

    public void UpdateMove()
    {
        if (_target == null)
        {
            _actor.state.SetState(ActorState.Die);
            return;
        }

        Vector3 dir = (_target.position - _actor.transform.position).normalized;
        _actor.dir = Vector3.Lerp(_actor.dir, dir, Time.deltaTime * 5f);

        _actor.transform.Translate(_actor.dir * _actor.moveSpeed * Time.deltaTime);

        _moveDistance += (_actor.transform.position - _prevPos).magnitude;
        _prevPos = _actor.transform.position;
        if (_moveDistance > _actor.distance)
        {
            _actor.state.SetState(ActorState.Die);
        }
    }

    public void Clear()
    {
        _prevPos = Vector3.zero;
        _moveDistance = 0;
    }
}
