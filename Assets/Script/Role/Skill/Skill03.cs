using System.Collections;
using UnityEngine;


public class Skill03 : Skill
{
    protected override IEnumerator coShot()
    {
        for (int i = 0; i < _shotCount; i++)
        {
            var role = _context.GetProjectile(_projectileData[0].id);
            var position = _actor.point.GetChild(0).transform.position;
            var projectile = _context.actorSpawner.Spawn<ActorProjectile, ProjectileDefinition>(role, position);
            projectile.Shot(_actor);
            projectile.onDied += HnadleDied;

            yield return new WaitForSeconds(_shotDelay);
        }

        yield return null;
    }

    private void HnadleDied(ActorBase actor)
    {
        var role = _context.GetProjectile(_projectileData[1].id);
        var position = actor.transform.position;
        var projectile = _context.actorSpawner.Spawn<ActorProjectile, ProjectileDefinition>(role, position);
        projectile.Shot(_actor);
    }
}
