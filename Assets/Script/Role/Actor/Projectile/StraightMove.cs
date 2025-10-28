using UnityEngine;


public class StraightMove : IProjectileMoveBehaviour
{
    private ActorProjectile _projectile;
    private Vector3 _prevPos;
    private float _moveDistance;

    public void Init(ActorProjectile projectile)
    {
        _projectile = projectile;
        _prevPos = projectile.transform.position;
        _moveDistance = 0;
    }

    public void UpdateMove()
    {
        Vector3 moveStep = _projectile.dir * _projectile.role.moveSpeed * Time.deltaTime;
        _projectile.transform.Translate(moveStep);

        _moveDistance += (_projectile.transform.position - _prevPos).magnitude;
        _prevPos = _projectile.transform.position;
        if (_moveDistance > _projectile.role.data.distance)
        {
            _projectile.state.SetState(ActorState.Die);
        }
    }

    public void Clear()
    {
        _prevPos = Vector3.zero;
    }
}