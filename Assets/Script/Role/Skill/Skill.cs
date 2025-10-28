using System.Collections;
using UnityEngine;


public class Skill : MonoBehaviour
{
    protected ActorBase _actor;

    public int ID { get; private set; }
    public string NAME { get; private set; }

    protected int _shotCount { get; private set; }
    protected float _shotDelay { get; private set; }
    protected float _reloadTime { get; private set; }
    protected ProjectileData[] _projectileData { get; private set; }

    protected float _shotTimer = 0f;

    public virtual void Update()
    {
        if (BattleManager.instance.IsBattleRun())
        {
            _shotTimer += Time.deltaTime;

            if (_shotTimer >= _reloadTime)
            {
                _shotTimer = 0f;

                Shot();
            }
        }
    }

    public virtual void Init(ActorBase actor, SkillData data)
    {
        _actor = actor;

        transform.SetParent(_actor.transform, false);
        transform.localPosition = Vector3.zero;

        ID = data.id;
        NAME = data.name;
        _shotCount = data.shotCount;
        _shotDelay = data.shotDelay;
        _reloadTime = data.reloadTime;
        _projectileData = data.projectileData;

        foreach (var item in _projectileData)
        {
            BattleManager.instance.actorManager.InitProjectile(item.id);
        }
    }

    protected void Shot()
    {
        StartCoroutine(coShot());
    }

    protected virtual IEnumerator coShot()
    {
        for (int i = 0; i < _shotCount; i++)
        {
            var role = ProjectileHander.instance.GetProjectileById(_projectileData[0].id);
            var parent = BattleManager.instance.battleScene.actorContainer;
            var position = _actor.point.GetChild(0).transform.position;
            var projectile = BattleManager.instance.actorManager.GetActor(role, parent, position) as ActorProjectile;
            projectile.Shot(_actor);

            yield return new WaitForSeconds(_shotDelay);
        }

        yield return null;
    }
}
