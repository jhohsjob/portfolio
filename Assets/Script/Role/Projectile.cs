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

    public override void Enter(object data = null)
    {
        base.Enter(data);

        var projectileData = _roleData as ProjectileData;

        _power = projectileData.power;
    }

    protected override void Move()
    {
        transform.localPosition += _dir * _moveSpeed * Time.deltaTime;
    }

    protected override void Die()
    {
        _bc.enabled = false;
        _dir = Vector3.zero;

        BattleManager.instance.roleManager.Retrieve(this);

        base.Die();
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
            if (body.role is Actor actor)
            {
                if (actor.team != team)
                {
                    Die();

                    actor.BeHit(_power);
                }
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
