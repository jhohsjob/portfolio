using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;


public class DashController
{
    private ActorBase _actor;
    private IAssetLoader _assetLoader;

    private Tween _dashTween;
    private float _distance = 1f;
    private float _speed;
    private int _maxCount;
    private int _currentCount;
    private float _cooldown;
    private float _cooldownTimer;

    private SpriteRenderer _sprite;
    private Material _dashMaterial;

    private bool _isOnCooldown => _cooldownTimer > 0f;
    private bool _isDashAvailable => _currentCount > 0 && !_isOnCooldown;

    public Action<float, float> onCooldownChanged;

    public void InitDependencies(ActorBase actor, IAssetLoader assetLoader)
    {
        _actor = actor;
        _assetLoader = assetLoader;
    }

    public void Init(float speed, int count, float cooldown, SpriteRenderer sprite)
    {
        _speed = speed;
        _maxCount = count;
        _currentCount = count;
        _cooldown = cooldown;
        _cooldownTimer = 0f;

        _sprite = sprite;
        _assetLoader.LoadMaterial("DashMat", mat =>
        {
            _dashMaterial = mat;
        });
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

    public void Dash(Transform transform, Vector2 lookDirection, Action callback)
    {
        if (_isDashAvailable == false)
        {
            return;
        }

        Vector3 targetPos = transform.position + (Vector3)lookDirection * _distance;

        _dashTween?.Kill();

        Material prev = _sprite.material;
        _sprite.material = _dashMaterial;

        _dashMaterial.SetVector("_Direction", new Vector4(lookDirection.x, lookDirection.y, 0f, 0f));
        _dashMaterial.SetFloat("_BlurStrength", 1f);
        _dashMaterial.SetFloat("_Alpha", 1f);

        IEnumeratorTool.instance.StartCoroutine(FadeOut(prev));

        _dashTween = transform.DOMove(targetPos, _speed).SetEase(Ease.Linear).OnComplete(() =>
        {
            _currentCount--;
            if (_currentCount <= 0)
            {
                _cooldownTimer = _cooldown;
            }

            callback?.Invoke();
        });
    }

    private IEnumerator FadeOut(Material prev)
    {
        float t = 0f;
        while (t < _speed)
        {
            t += Time.deltaTime;
            _dashMaterial.SetFloat("_BlurStrength", Mathf.Lerp(1f, 0f, t));
            _dashMaterial.SetFloat("_Alpha", Mathf.Lerp(1f, 0f, t));
            yield return null;
        }

        _sprite.material = prev;
    }

    public void OnElementLevelUp()
    {
        _distance += 0.1f;
    }
}