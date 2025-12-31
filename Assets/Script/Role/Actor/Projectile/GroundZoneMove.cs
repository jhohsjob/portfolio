using UnityEngine;


public class GroundZoneMove : IProjectileMoveBehaviour
{
    private ActorProjectile _projectile;
    private float _timer;
    private float _damageTimer;
    private float _damageDelay = 0.5f;

    public void Init(ActorProjectile projectile)
    {
        _projectile = projectile;
        _timer = 0f;
        _damageTimer = 0f;

        var pos = _projectile.transform.position;
        _projectile.transform.position = pos;
    }

    public void UpdateMove()
    {
        _timer += Time.deltaTime;

        if (_timer >= _projectile.Role.distance)
        {
            _projectile.State.SetState(ActorState.Die);
        }

        _damageTimer += Time.deltaTime;
        if (_damageTimer >= _damageDelay)
        {
            _projectile.ResetCollider();
            _damageTimer = 0f;
        }
    }

    public void Clear() { }
}
