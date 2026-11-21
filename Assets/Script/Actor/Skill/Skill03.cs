using System.Collections;
using UnityEngine;


public class Skill03 : Skill
{
    protected override IEnumerator coShot()
    {
        for (int i = 0; i < _fireCount; i++)
        {
            var role = _env.GetProjectile(_projectileData[0].Id);
            var position = _actor.muzzlePos;
            var projectile = _env.spawner.Spawn<ActorProjectile, ProjectileDefinition>(role, position);
            projectile.Shot(_actor);
            projectile.onDied += HandleDied;

            yield return new WaitForSeconds(_shotDelay);
        }

        yield return null;
    }

    private void HandleDied(ActorBase actor)
    {
        var role = _env.GetProjectile(_projectileData[1].Id);
        var position = actor.transform.position;
        var projectile = _env.spawner.Spawn<ActorProjectile, ProjectileDefinition>(role, position);
        projectile.Shot(_actor);
    }
}
