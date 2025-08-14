using System.Collections;
using UnityEngine;


public class Skill : MonoBehaviour
{
    protected ActorBase _actor;

    public int ID { get; private set; }
    public string NAME { get; private set; }

    protected int shotCount { get; private set; }
    protected float shotDelay { get; private set; }
    protected float reloadTime { get; private set; }
    protected int projectileId { get; private set; }

    protected float _shotTimer = 0f;
    protected int _shotCount = 0;

    void Update()
    {
        if (BattleManager.instance.battleStatus == BattleStatus.Run)
        {
            _shotTimer += Time.deltaTime;

            if (_shotTimer >= reloadTime)
            {
                _shotTimer = 0f;
                _shotCount++;

                Shot();
            }
        }
    }

    public void Init(ActorBase actor, SkillData data)
    {
        _actor = actor;

        transform.SetParent(_actor.transform, false);
        transform.localPosition = Vector3.zero;

        ID = data.id;
        NAME = data.name;
        shotCount = data.shotCount;
        shotDelay = data.shotDelay;
        reloadTime = data.reloadTime;
        projectileId = data.projectileId;

        BattleManager.instance.actorManager.InitProjectile(projectileId);
    }

    protected void Shot()
    {
        StartCoroutine(coShot());
    }

    protected IEnumerator coShot()
    {
        for (int i = 0; i < shotCount; i++)
        {
            var role = ProjectileHander.instance.GetProjectileById(projectileId);
            var parent = BattleManager.instance.battleScene.actorContainer;
            var position = _actor.transform.position;
            var projectile = BattleManager.instance.actorManager.GetActor(role, parent, position) as ActorProjectile;
            projectile.Shot(_actor, _actor.point.transform.position);

            yield return new WaitForSeconds(shotDelay);
        }

        yield return null;
    }
}
