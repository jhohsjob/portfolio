using UnityEngine;


public class PlayerMoveController
{
    private Player _player;
    private Vector2 _extents;

    private Vector2 _moveDirection = Vector2.zero;
    public Vector2 moveDirection => _moveDirection;
    private Vector2 _lookDirection = Vector2.right;
    public Vector2 lookDirection => _lookDirection;

    private float _elementMoveSpeed = 0f;
    private float _moveSpeed => (_player?.Role?.moveSpeed ?? 0f) + _elementMoveSpeed;

    public void Init(Player player)
    {
        _player = player;
        _extents = _player.Body.sprite.bounds.extents;
    }

    public void Move()
    {
        if (_moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(_moveDirection.y, _moveDirection.x) * Mathf.Rad2Deg;
            _player.point.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        _player.transform.Translate(_moveDirection * _moveSpeed * Time.deltaTime);

        var pos = _player.transform.position;
        var mapBounds = BattleManager.instance.mapBounds;

        pos.x = Mathf.Clamp(pos.x, mapBounds.min.x + _extents.x, mapBounds.max.x - _extents.x);
        pos.y = Mathf.Clamp(pos.y, mapBounds.min.y + _extents.y, mapBounds.max.y - _extents.y);

        _player.transform.position = pos;
    }

    public void SetMoveDirection(Vector2 input)
    {
        _moveDirection = input;

        if (_moveDirection != Vector2.zero)
        {
            _lookDirection = _moveDirection;
        }

        _player.Animator.SetFloat("Speed", _moveDirection == Vector2.zero ? 0f : 1f);

        if (_moveDirection.x != 0f)
        {
            _player.Body.FlipX(_moveDirection.x);
        }

        _player.State.SetState(_moveDirection == Vector2.zero ? ActorState.Idle : ActorState.Move);
    }

    public void OnElementLevelUp()
    {
        _elementMoveSpeed += 0.5f;
    }
}
