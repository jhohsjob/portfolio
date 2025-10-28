using System;
using UnityEngine;


public class Body : MonoBehaviour
{
    private ActorBase _actor;
    public ActorBase actor => _actor;

    private SpriteRenderer _sprite;
    public SpriteRenderer sprite => _sprite;
    
    private Renderer _particle;

    private Action<Body> _cbTriggerEnter;
    public event Action<Body> cbTriggerEnter
    {
        add { _cbTriggerEnter -= value; _cbTriggerEnter += value; }
        remove { _cbTriggerEnter -= value; }
    }

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _particle = GetComponent<Renderer>();
    }

    public  void Init(ActorBase actor)
    {
        _actor = actor;
    }

    public void Enter(RoleType roleType)
    {
        if (_sprite != null)
        {
            _sprite.sortingOrder = ((int)roleType) + BattleManager.instance.actorManager.GetNextOrderInLayer();
        }

        if (_particle != null)
        {
            _particle.sortingOrder = ((int)roleType) + BattleManager.instance.actorManager.GetNextOrderInLayer();
        }
    }

    public void FlipX(float dirX)
    {
        if (_sprite != null && _sprite.flipX != dirX < 0f)
        {
            _sprite.flipX = dirX < 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log("OnTriggerEnter : " + other);

        if (other.TryGetComponent(out Body body))
        {
            _cbTriggerEnter?.Invoke(body);
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
