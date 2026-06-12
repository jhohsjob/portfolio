using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActorView : MonoBehaviour
{
    private Transform _point;
    private Transform _muzzle;

    private Body _body;
    private RenderCheck _renderCheck;
    private Animator _animator;
    private FlashShader _flashShader;
    private Collider2D _collider;

    private bool _isVisible;

    public Vector2 extents => _body != null ? _body.mainRenderer.bounds.extents : Vector2.zero;
    public Vector3 muzzlePos => _point != null ? _muzzle.position : Vector3.zero;
    public Vector3 muzzleDir => _point != null ? _point.right : Vector3.zero;
    public Action<Body> onBodyTriggerEnter;

    private readonly Dictionary<Type, ActorSubView> _views = new();

    private void Awake()
    {
        _point = transform.Find("point");
        if (_point != null)
        {
            _muzzle = _point.GetChild(0).transform;
        }
    }

    public void Initialize(ActorBase actor, RoleBase role)
    {
        var body = Instantiate(role.original, transform);
        body.transform.localScale = Vector3.one / 2f;
        body.transform.localPosition = role.resourceOffset;

        _body = body.AddComponent<Body>();
        _body.Init(actor);

        _renderCheck = body.AddComponent<RenderCheck>();
        _renderCheck.SetRenderer(_body.mainRenderer);

        _animator = body.GetComponent<Animator>();
        _flashShader = body.GetComponent<FlashShader>();
        _collider = body.GetComponent<Collider2D>();

        foreach (var view in _views.Values)
        {
            view.SetSprite(_body.mainRenderer);
        }

        _isVisible = false;
        EnableAnimation(false);
        SetCollider(false);
    }

    public void Bind()
    {
        _body.OnTriggerEntered += OnBodyTriggerEnter;
        _renderCheck.onVisibleChanged += OnVisibleChanged;
    }

    public void Unbind()
    {
        _body.OnTriggerEntered -= OnBodyTriggerEnter;
        _renderCheck.onVisibleChanged -= OnVisibleChanged;
    }

    public void Enter(int order)
    {
        _body.Enter(order);

        SetCollider(true);
        // EnableAnimation(true);
    }

    public void Die()
    {
        _body.Die();

        SetCollider(false);
        EnableAnimation(false);
    }

    public void PlayFlash()
    {
        if (_isVisible == false)
        {
            return;
        }

        _flashShader?.FlashOnce();
    }

    public void SetCollider(bool enable)
    {
        if (_collider != null)
        {
            _collider.enabled = enable;
        }
    }

    public void ResetCollider(float time = 0.5f)
    {
        StartCoroutine(CoResetCollider(time));
    }

    IEnumerator CoResetCollider(float time)
    {
        SetCollider(false);
        yield return WaitCache.Get(time);
        SetCollider(true);
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetLookDirection(Quaternion lookDirection)
    {
        _point.rotation = lookDirection;
    }

    public void SetFlip(Vector2 dir)
    {
        _body.FlipX(dir.x);
    }

    public void EnableAnimation(bool enabled)
    {
        if (_animator != null)
        {
            _animator.enabled = enabled;
        }
    }

    public void SetAnimation(string state)
    {
        _animator?.Play(state);
    }

    public void DeadAnimation()
    {
        _animator?.SetBool("Dead", true);
    }

    public void MoveAnimation(bool isMoving)
    {
        _animator.SetFloat("Speed", isMoving == true ? 1f : 0f);
    }

    private void OnBodyTriggerEnter(Body other)
    {
        onBodyTriggerEnter?.Invoke(other);
    }

    private void OnVisibleChanged(bool isVisible)
    {
        _isVisible = isVisible;

        _body.SetVisible(isVisible);
        EnableAnimation(isVisible);
    }

    public T AddView<T>() where T : ActorSubView
    {
        var view = gameObject.AddComponent<T>();

        _views[typeof(T)] = view;

        return view;
    }

    public void AddView<T>(T view) where T : ActorSubView
    {
        _views[typeof(T)] = view;
    }

    public T GetView<T>() where T : ActorSubView
    {
        return _views.TryGetValue(typeof(T), out var view) ? view as T : null;
    }
}