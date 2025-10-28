using UnityEngine;


public struct OrbitMoveData
{
    public float radius;
    public float startAngle;
    public float angularSpeed;
}

public class OrbitMove : IProjectileMoveBehaviour
{
    private ActorProjectile _projectile;
    private Transform _center;
    private float _radius;
    private float _angle;
    private float _angularSpeed;

    public OrbitMove(Transform center, OrbitMoveData data)
    {
        _center = center;
        _radius = data.radius;
        _angle = data.startAngle;
        _angularSpeed = data.angularSpeed;
    }

    public void Init(ActorProjectile projectile)
    {
        _projectile = projectile;
    }

    public void UpdateMove()
    {
        if (_projectile == null || _center == null)
        {
            return;
        }

        float angle = _angle + Time.time * _angularSpeed;

        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * _radius;
        _projectile.transform.position = _center.position + offset;
        _projectile.transform.right = offset.normalized;
    }

    public void Clear()
    {
        _projectile = null;
        _center = null;
    }
}
