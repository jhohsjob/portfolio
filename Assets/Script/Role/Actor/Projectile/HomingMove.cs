using UnityEngine;

public class HomingMove : IProjectileMoveBehaviour
{
    private ActorProjectile _projectile;
    private Transform _target;
    private Vector3 _prevPos;
    private float _moveDistance;

    public void Init(ActorProjectile projectile)
    {
        _projectile = projectile;
        _target = BattleManager.instance.enemyManager.GetNearestEnemy(projectile.transform.position, projectile.dir, 10f, 120f);
        _prevPos = projectile.transform.position;
        _moveDistance = 0;
    }

    public void UpdateMove()
    {
        if (_target == null)
        {
            _projectile.state.SetState(ActorState.Die);
            return;
        }

        Vector3 dir = (_target.position - _projectile.transform.position).normalized;
        _projectile.dir = Vector3.Lerp(_projectile.dir, dir, Time.deltaTime * 5f);

        _projectile.transform.Translate(_projectile.dir * _projectile.role.moveSpeed * Time.deltaTime);

        _moveDistance += (_projectile.transform.position - _prevPos).magnitude;
        _prevPos = _projectile.transform.position;
        if (_moveDistance > _projectile.role.data.distance)
        {
            _projectile.state.SetState(ActorState.Die);
        }
    }

    public void Clear()
    {
        _projectile = null;
    }
}
