using System;
using UnityEngine;


public class Body : MonoBehaviour
{
    private ActorBase _actor;
    public ActorBase actor => _actor;

    private SpriteRenderer _sprite;
    public SpriteRenderer sprite => _sprite;
    
    private Renderer _particle;

    public event Action<Body> OnTriggerEntered;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _particle = GetComponent<Renderer>();
    }

    public  void Init(ActorBase actor)
    {
        _actor = actor;
    }

    public void Enter(int sortingOrder)
    {
        if (_sprite != null)
        {
            _sprite.sortingOrder = sortingOrder;
        }

        if (_particle != null)
        {
            _particle.sortingOrder = sortingOrder;
        }
    }

    public void FlipX(float dirX)
    {
        bool shouldFlip = dirX < 0f;

        if (_sprite != null && _sprite.flipX != shouldFlip)
        {
            _sprite.flipX = shouldFlip;
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
