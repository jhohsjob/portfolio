using UnityEngine;


public class ActorProjectile : Actor<Projectile, ProjectileData>
{
    private ActorBase _actor;
    public Vector3 dir { get; set; }
    
    private IProjectileMoveBehaviour _moveBehaviour;

    public override void Init(Projectile role)
    {
        base.Init(role);

        _body.cbTriggerEnter += OnBodyTriggerEnter;
    }

    public override void Enter(object data = null)
    {
        base.Enter(data);

        _moveBehaviour = CreateMoveBehaviour(_role.data.moveType, data);
        _moveBehaviour.Init(this);
    }

    void Update()
    {
        _moveBehaviour?.UpdateMove();
    }

    protected override void Die()
    {
        base.DieAfter();

        _moveBehaviour?.Clear();
        _moveBehaviour = null;

        BattleManager.instance.actorManager.Return(this);

        base.Die();
    }

    public void Shot(ActorBase actor, object data = null)
    {
        _actor = actor;
        team = actor.team;
        dir = actor.point.right;

        Enter(data);
    }

    private void OnBodyTriggerEnter(Body other)
    {
        if (other.TryGetComponent(out Body body))
        {
            if (body.actor is ActorBase actor)
            {
                if (actor.team != team && actor.team != Team.None)
                {
                    if (role.beHitDie == true)
                    {
                        _state.SetState(ActorState.Die);
                    }
                    actor.BeHit(_role.power);
                }
            }
        }
    }

    private IProjectileMoveBehaviour CreateMoveBehaviour(ProjectileMoveType type, object data = null)
    {
        return type switch
        {
            ProjectileMoveType.Straight => new StraightMove(),
            ProjectileMoveType.Orbit when data is OrbitMoveData moveData => new OrbitMove(_actor.transform, moveData),
            ProjectileMoveType.Homing => new HomingMove(),
            ProjectileMoveType.GroundZone => new GroundZoneMove(),

            _ => new StraightMove()
        };
    }

}
