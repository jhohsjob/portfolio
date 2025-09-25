using UnityEngine;


public class ActorProjectile : Actor<Projectile, ProjectileData>
{
    private Vector3 _dir = Vector3.zero;

    private float _moveDistance;
    private Vector3 _prevPos;

    protected override void Awake()
    {
    }

    void Update()
    {
        if (_dir != Vector3.zero)
        {
            Move();
        }
    }

    public override void Init(Projectile role)
    {
        base.Init(role);

        _moveDistance = 0f;
        _prevPos = Vector3.zero;

        _body.cbTriggerEnter += OnBodyTriggerEnter;
    }

    public override void Enter(object data = null)
    {
        base.Enter(data);
        
        _moveDistance = 0f;
        _prevPos = transform.position;
    }

    protected override void Move()
    {
        Vector3 moveStep = _dir * _role.moveSpeed * Time.deltaTime;
        transform.Translate(moveStep);

        _moveDistance += (transform.position - _prevPos).magnitude;

        _prevPos = transform.position;

        if (_moveDistance > _role.data.distance)
        {
            Die();
        }
    }

    protected override void Die()
    {
        // Debug.Log("Die : " + ID);

        _dir = Vector3.zero;
        _moveDistance = 0f;
        _prevPos = Vector3.zero;

        BattleManager.instance.actorManager.Return(this);

        base.Die();
    }

    public void Shot(ActorBase actor)
    {
        Enter();

        team = actor.team;
        _dir = actor.point.right;
    }

    private void OnBodyTriggerEnter(Body other)
    {
        // Debug.Log("OnTriggerEnter : " + other);

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
}
