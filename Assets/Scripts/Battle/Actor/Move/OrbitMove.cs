using UnityEngine;


public class OrbitMove : IMoveBehaviour
{
    private ActorBase _actor;
    private Transform _center;
    private float _radius;
    private float _angle;
    private float _angularSpeed;

    public void Init(ActorBase actor)
    {
        _actor = actor;
    }

    public void Setup(Transform center, float radius, float startAngle, float angularSpeed)
    {
        _center = center;
        _radius = radius;
        _angle = startAngle;
        _angularSpeed = angularSpeed;
    }

    public void UpdateMove()
    {
        if (_actor == null || _center == null)
        {
            return;
        }

        float angle = _angle + Time.time * _angularSpeed;

        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * _radius;
        _actor.transform.position = _center.position + offset;
        _actor.transform.right = offset.normalized;
    }

    public void Clear()
    {
        _center = null;
    }
}
