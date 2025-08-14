using UnityEngine;


public class ActorProjectile : Actor<Projectile, ProjectileData>
{
    private BoxCollider _bc;

    private Vector3 _dir = Vector3.zero;

    protected override void Awake()
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

    protected override void Move()
    {
        transform.localPosition += _dir * _role.moveSpeed * Time.deltaTime;
    }

    protected override void Die()
    {
        _bc.enabled = false;
        _dir = Vector3.zero;

        BattleManager.instance.actorManager.Return(this);

        base.Die();
    }

    public void Shot(ActorBase actor, Vector3 position)
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
            if (body.actor is ActorBase actor)
            {
                if (actor.team != team && actor.team != Team.None)
                {
                    Die();

                    actor.BeHit(_role.power);
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
