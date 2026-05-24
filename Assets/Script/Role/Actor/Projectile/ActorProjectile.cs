using UnityEngine;


public class ActorProjectile : Actor<Projectile, ProjectileDefinition>
{
    private ActorBase _owner;
    private Vector3 _dir;
    public override Vector3 dir { get { return _dir; } set { _dir = value; } }
    public override float distance => _role.distance;
    
    public override void Init(Projectile role)
    {
        base.Init(role);

        _body.OnTriggerEntered += OnBodyTriggerEnter;
    }

    public void Shot(ActorBase owner)
    {
        _owner = owner;
        team = owner.team;
        _dir = owner.point.right;
        
        _state.SetState(ActorState.Move);
    }

    private void OnBodyTriggerEnter(Body other)
    {
        if (other.TryGetComponent(out Body body))
        {
            if (body.actor is ActorBase actor)
            {
                if (actor.team != team && actor.team != Team.None)
                {
                    if (_role.beHitDie == true)
                    {
                        _state.SetState(ActorState.Die);
                    }
                    actor.BeHit(_role.power);
                }
            }
        }
    }
}