using System.Collections;
using UnityEngine;


public class MultiShotSkill : Skill
{
    protected override IEnumerator coShot()
    {
        float startAngle = -_spreadAngle * (_multiShotCount - 1) * 0.5f;

        int projectileId = _projectileData[0].id;

        for (int i = 0; i < _fireCount; i++)
        {
            for (int j = 0; j < _multiShotCount; j++)
            {
                var role = _env.GetProjectile(projectileId);
                var position = _actor.muzzlePos;
                var projectile = _env.spawner.Spawn<ActorProjectile, ProjectileDefinition>(role, position);

                float angle = startAngle + _spreadAngle * j;
                Vector3 dir = Quaternion.Euler(0, 0, angle) * _actor.muzzleDir;

                if (projectile.moveBehaviour is HomingMove homing)
                {
                    var target = _env.GetNearestEnemy(projectile.transform.position, projectile.dir, 10f, 120f);
                    homing.Setup(target);
                }
                projectile.Shot(_actor, dir);
            }

            yield return new WaitForSeconds(_shotDelay);
        }

        yield return null;
    }
}
