using System;
using UnityEngine;


public class Body : MonoBehaviour
{
    private ActorBase _actor;
    public ActorBase actor => _actor;

    private Action<Body> _cbTriggerEnter;
    public event Action<Body> cbTriggerEnter
    {
        add { _cbTriggerEnter -= value; _cbTriggerEnter += value; }
        remove { _cbTriggerEnter -= value; }
    }

    private void Awake()
    {
        _actor = transform.parent.GetComponent<ActorBase>();
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
