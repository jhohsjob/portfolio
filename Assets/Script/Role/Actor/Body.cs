using System;
using UnityEngine;


public class Body : MonoBehaviour
{
    private ActorBase _actor;
    public ActorBase actor => _actor;

    //private SpriteRenderer _sprite;
    //public SpriteRenderer sprite => _sprite;

    //private ParticleSystemRenderer _particle;

    private Renderer _renderer;
    public Renderer mainRenderer => _renderer;

    private SpriteRenderer _sprite;
    private ParticleSystem _particle;
    private ParticleSystem.EmissionModule _emission;

    public event Action<Body> OnTriggerEntered;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _particle = GetComponent<ParticleSystem>();
        if (_particle != null)
        {
            _emission = _particle.emission;
        }
        //_sprite = GetComponent<SpriteRenderer>();
        //_particle = GetComponent<ParticleSystemRenderer>();
    }

    public  void Init(ActorBase actor)
    {
        _actor = actor;

        SetVisible(false);
    }

    public void Enter(int sortingOrder)
    {
        if (_renderer != null)
        {
            _renderer.sortingOrder = sortingOrder;
        }

        //if (_sprite != null)
        //{
        //    _sprite.sortingOrder = sortingOrder;
        //}

        //if (_particle != null)
        //{
        //    _particle.sortingOrder = sortingOrder;
        //}

        // SetVisible(true);
    }

    public void Die()
    {
        SetVisible(false);
    }

    public void FlipX(float dirX)
    {
        if (_sprite == null && _renderer is SpriteRenderer sprite)
        {
            _sprite = sprite;
        }

        bool shouldFlip = dirX < 0f;

        if (_sprite != null && _sprite.flipX != shouldFlip)
        {
            _sprite.flipX = shouldFlip;
        }
    }

    public void SetVisible(bool isVisible)
    {
        if (_particle != null)
        {
            _emission.enabled = isVisible;
            if (isVisible == true)
            {
                _particle.Play();
            }
            else
            {
                _particle.Pause();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log("OnTriggerEnter : " + other);

        if (other.TryGetComponent(out Body body))
        {
            OnTriggerEntered?.Invoke(body);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Debug.Log("OnTriggerStay : " + other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Debug.Log("OnTriggerExit : " + other);
    }
}
