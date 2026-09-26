using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputSource : IInputSource
{
    private Vector2 _moveDirection;
    public Vector2 MoveDirection => _moveDirection;

    private Vector2 _lookDirection = Vector2.right;
    public Vector2 LookDirection => _lookDirection;

    public void UpdateInput()
    {
        // polling 방식이면 여기서 입력 읽기
    }

    public void SetMoveDirection(Vector2 input)
    {
        _moveDirection = input;

        if (_moveDirection != Vector2.zero)
        {
            _lookDirection = _moveDirection.normalized;
        }
    }

    public void OnMove(Vector2 value)
    {
        SetMoveDirection(value);
    }
}