using System;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;


public class DirectionMove : IMoveBehaviour
{
    private ActorBase _actor;

    private IInputSource _input;

    private Func<Bounds> _getMapBounds;
    private Bounds _mapBounds => _getMapBounds?.Invoke() ?? new Bounds();

    private Vector2 _extents;

    private float _moveSpeed => _actor?.moveSpeed ?? 0f;

    public void Init(ActorBase actor)
    {
        _actor = actor;

        _extents = _actor.extents;
    }

    public void Setup(IInputSource input, Func<Bounds> getMapBounds)
    {
        _input = input;
        _getMapBounds = getMapBounds;
    }

    public void UpdateMove()
    {
        Vector2 moveDirection = _input.MoveDirection;

        if (moveDirection == Vector2.zero)
        {
            // _actor.animator.SetFloat("Speed", 0f);
            _actor.SetMoving(false);
            return;
        }

        Rotate(moveDirection);

        Move(moveDirection);

        ClampPosition();

        Flip(moveDirection);

        // _actor.animator.SetFloat("Speed", 1f);
        _actor.SetMoving(true);
    }

    private void Rotate(Vector2 dir)
    {
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var lookDirection = Quaternion.Euler(0f, 0f, angle);
        _actor.SetLookDirection(lookDirection);
    }

    private void Move(Vector2 dir)
    {
        _actor.transform.Translate(dir * _moveSpeed * Time.deltaTime);
    }

    private void ClampPosition()
    {
        var pos = _actor.transform.position;

        pos.x = Mathf.Clamp(pos.x, _mapBounds.min.x + _extents.x, _mapBounds.max.x - _extents.x);

        pos.y = Mathf.Clamp(pos.y, _mapBounds.min.y + _extents.y, _mapBounds.max.y - _extents.y);

        _actor.transform.position = pos;
    }

    private void Flip(Vector2 dir)
    {
        if (dir.x != 0f)
        {
            _actor.SetFlip(dir);
        }
    }

    public void Clear()
    {
    }
}