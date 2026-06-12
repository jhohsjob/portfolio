using DG.Tweening;
using System;
using UnityEngine;


public class DashController
{
    private ActorBase _actor;

    private Tween _dashTween;
    private float _distance = 1f;
    private float _speed;
    private int _maxCount;
    private int _currentCount;
    private float _cooldown;
    private float _cooldownTimer;

    private bool _isOnCooldown => _cooldownTimer > 0f;
    private bool _isDashAvailable => _currentCount > 0 && !_isOnCooldown;

    public Action<float, float> onCooldownChanged;

    public void InitDependencies(ActorBase actor)
    {
        _actor = actor;
    }

    public void Init(float speed, int count, float cooldown)
    {
        _speed = speed;
        _maxCount = count;
        _currentCount = count;
        _cooldown = cooldown;
        _cooldownTimer = 0f;
    }

    public void Update(float deltaTime)
    {
        if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= deltaTime;
            if (_cooldownTimer <= 0f)
            {
                _currentCount = _maxCount;
                _cooldownTimer = 0f;
            }

            onCooldownChanged?.Invoke(_cooldownTimer, _cooldown);
        }
    }

    public void TryDash()
    {
        if (_isDashAvailable == false)
        {
            return;
        }

        _actor.state.SetState(ActorState.Dash);
    }

    public bool Dash(Transform transform, Vector2 lookDirection, Action callback)
    {
        if (_isDashAvailable == false)
        {
            return false;
        }

        Vector3 targetPos = transform.position + (Vector3)lookDirection * _distance;

        _dashTween?.Kill();

        _dashTween = transform.DOMove(targetPos, _speed).SetEase(Ease.Linear).OnComplete(() =>
        {
            _currentCount--;
            if (_currentCount <= 0)
            {
                _cooldownTimer = _cooldown;
            }

            callback?.Invoke();
        });

        return true;
    }

    public void OnElementLevelUp()
    {
        _distance += 0.1f;
    }
}