using UnityEngine;


public class ActorProjectile : Actor<Projectile>
{
    private ActorBase _owner;

    private bool _beHitDie;
    private float _power;

    public override void Init(RoleBase role)
    {
        base.Init(role);

        if (role is not Projectile projectile)
        {
            return;
        }

        distance = projectile.distance;
        _beHitDie = projectile.beHitDie;
        _power = projectile.power;
    }

    public void Shot(ActorBase owner, Vector3? direction = null)
    {
        _owner = owner;
        team = owner.team;
        dir = direction ?? owner.muzzleDir;

        _state.SetState(ActorState.Move);
    }

    protected override void OnBodyTriggerEnter(Body other)
    {
        var target = other.actor;
        if (target.team != team && target.team != Team.None)
        {
            DamageResult result = CalculateDamage(target);
            target.BeHit(result.damage);

            if (_beHitDie == true)
            {
                _state.SetState(ActorState.Die);
            }
        }
    }

    private DamageResult CalculateDamage(ActorBase target)
    {
        DamageRequest request = new DamageRequest(_owner.stat, target.stat, _power);

        return DamageCalculator.Calculate(request);
    }
}