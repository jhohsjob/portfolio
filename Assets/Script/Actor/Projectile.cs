using TMPro;
using UnityEngine;


public class Projectile : Actor
{
    public override Enums.Team team { get; set; }
    protected override float _moveSpeed { get; set; } = 5f;

    private Vector3 _dir = Vector3.zero;
    
    void Update()
    {
        if (_dir != Vector3.zero)
        {
            Move();
        }
    }

    public void Init(Actor actor, Transform point)
    {
        team = actor.team;
        transform.position = point.position;

        var dir = point.position - actor.transform.position;
        dir.y = 0f;
        var rot = Quaternion.LookRotation(dir);
        _dir = dir.normalized;
        transform.rotation = rot;
    }

    protected override void Move()
    {
        transform.localPosition += _dir * _moveSpeed * Time.deltaTime;
    }

    public override void Die()
    {
        SpawnManager.instance.Retrieve(Enums.SpawnType.Projectile, gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("OnTriggerEnter : " + other);
        
        if (other.CompareTag("Outside") == true)
        {
            Die();
            return;
        }

        if (other.TryGetComponent(out Body body))
        {
            if (body.actor.team != team)
            {
                body.actor.Die();

                Die();
            }
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
