using UnityEngine;


public interface IInputSource
{
    Vector2 MoveDirection { get; }
    Vector2 LookDirection { get; }

    void UpdateInput();
}