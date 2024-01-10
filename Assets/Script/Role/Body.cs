using System;
using UnityEngine;


public class Body : MonoBehaviour
{
    private Role _role;
    public Role role => _role;

    private Action<Body> _cbTriggerEnter;
    public event Action<Body> cbTriggerEnter
    {
        add { _cbTriggerEnter -= value; _cbTriggerEnter += value; }
        remove { _cbTriggerEnter -= value; }
    }

    private void Awake()
    {
        _role = transform.parent.GetComponent<Role>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("OnTriggerEnter : " + other);

        if (other.TryGetComponent(out Body body))
        {
            _cbTriggerEnter?.Invoke(body);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Debug.Log("OnTriggerStay : " + other);
    }

    private void OnTriggerExit(Collider other)
    {
        // Debug.Log("OnTriggerExit : " + other);
    }
}
