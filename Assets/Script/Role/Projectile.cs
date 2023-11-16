using UnityEngine;


public class Projectile : Role
{
    private BoxCollider _bc;

    protected float _power = 0f;

    private Vector3 _dir = Vector3.zero;

    private void Awake()
    {
        _bc = GetComponent<BoxCollider>();
        _bc.enabled = false;
    }

    void Update()
    {
        if (_dir != Vector3.zero)
        {
            Move();
        }
    }

    public override void Init(RoleData roleData)
    {
        var data = roleData as ProjectileData;
        if (data == null)
        {
            return;
        }
        
        base.Init(roleData);
    }

    public override void Enter()
    {
        base.Enter();

        var data = _roleData as ProjectileData;

        _power = data.power;
    }

    protected override void Move()
    {
        transform.localPosition += _dir * _moveSpeed * Time.deltaTime;
    }

    public void Shot(Actor actor, Vector3 position)
    {
        Enter();

        _bc.enabled = true;

        team = actor.team;

        var dir = position - actor.transform.position;
        dir.y = 0f;
        var rot = Quaternion.LookRotation(dir);
        _dir = dir.normalized;
        transform.rotation = rot;
    }

    public override void Die()
    {
        _bc.enabled = false;
        _dir = Vector3.zero;

        GameManager.instance.roleManager.Retrieve(this);

        base.Die();
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
                Die();

                body.actor.BeHit(_power);
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
