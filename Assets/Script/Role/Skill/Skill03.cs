using System.Collections;
using UnityEngine;


public class Skill03 : Skill
{
    protected override IEnumerator coShot()
    {
        for (int i = 0; i < _shotCount; i++)
        {
            var role = ProjectileHander.instance.GetProjectileById(_projectileData[0].id);
            var parent = BattleManager.instance.battleScene.actorContainer;
            var position = _actor.point.GetChild(0).transform.position;
            var projectile = BattleManager.instance.actorManager.GetActor(role, parent, position) as ActorProjectile;
            projectile.Shot(_actor);
            projectile.cbDie += OnDie;

            yield return new WaitForSeconds(_shotDelay);
        }

        yield return null;
    }

    private void OnDie(ActorBase actor)
    {
        var role = ProjectileHander.instance.GetProjectileById(_projectileData[1].id);
        var parent = BattleManager.instance.battleScene.actorContainer;
        var position = actor.transform.position;
        var projectile = BattleManager.instance.actorManager.GetActor(role, parent, position) as ActorProjectile;
        projectile.Shot(_actor);
    }
}
