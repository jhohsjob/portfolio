using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    public abstract Enums.Team team { get; set; }
    protected abstract float _moveSpeed { get; set; }

    protected Vector3 _vBorder = Vector3.zero;

    protected abstract void Move();
    public abstract void Die();
}
