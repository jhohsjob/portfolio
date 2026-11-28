using System.Collections;
using UnityEngine;


public class Skill01 : Skill
{
    protected override IEnumerator coShot()
    {
        for (int i = 0; i < _fireCount; i++)
        {
            var role = _env.GetProjectile(_projectileData[0].Id);
            var position = _actor.muzzlePos;
            var projectile = _env.spawner.Spawn<ActorProjectile, ProjectileDefinition>(role, position);
            if (projectile.moveBehaviour is HomingMove homing)
            {
                var target = _env.GetNearestEnemy(projectile.transform.position, projectile.dir, 10f, 120f);
                homing.Setup(target);
            }
            projectile.Shot(_actor);

            yield return new WaitForSeconds(_shotDelay);
        }

        yield return null;
    }
}
