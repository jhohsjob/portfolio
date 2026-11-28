using UnityEngine;


public class StraightMove : IMoveBehaviour
{
    private ActorBase _actor;
    private Vector3 _prevPos;
    private float _moveDistance;

    public void Init(ActorBase actor)
    {
        _actor = actor;
        _prevPos = Vector3.zero;
        _moveDistance = 0;
    }

    public void UpdateMove()
    {
        Vector3 moveStep = _actor.dir * _actor.moveSpeed * Time.deltaTime;
        _actor.transform.Translate(moveStep);

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