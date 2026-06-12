using System;
using System.Collections;
using UnityEngine;


public class SkillContext
{
    public IBattleState battleState;
    public IActorSpawner actorSpawner;
    public Func<int, Projectile> GetProjectile;
    public Func<Vector3, Vector3, float, float, Transform> GetNearestEnemy;
}

public class Skill : MonoBehaviour
{
    protected SkillContext _context;

    protected ActorBase _actor;

    public int ID { get; private set; }
    public string NAME { get; private set; }

    protected int _shotCount { get; private set; }
    protected float _shotDelay { get; private set; }
    protected float _reloadTime { get; private set; }
    protected ProjectileDefinition[] _projectileData { get; private set; }

    protected float _shotTimer = 0f;

    public virtual void Update()
    {
        if (_context.battleState.IsRunning() == true)
        {
            _shotTimer += Time.deltaTime;

            if (_shotTimer >= _reloadTime)
            {
                _shotTimer = 0f;

                Shot();
            }
        }
    }

    public virtual void Init(ActorBase actor, SkillData data, SkillContext context)
    {
        _actor = actor;
        _context = context;

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
            var role = ProjectileManager.instance.GetProjectileById(item.id);
            _context.actorSpawner.InitPool(role);
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
            var role = _context.GetProjectile(_projectileData[0].id);
            var position = _actor.muzzlePos;
            var projectile = _context.actorSpawner.Spawn<ActorProjectile, ProjectileDefinition>(role, position);
            projectile.Shot(_actor);

            yield return new WaitForSeconds(_shotDelay);
        }

        yield return null;
    }
}
