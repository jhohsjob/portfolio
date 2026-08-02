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

    protected int _fireCount { get; private set; }
    protected int _multiShotCount { get; private set; }
    protected float _shotDelay { get; private set; }
    protected float _reloadTime { get; private set; }
    protected ProjectileDefinition[] _projectileData { get; private set; }

    protected float _shotTimer = 0f;

    [SerializeField, ReadOnly]
    protected bool _isUsed = false;

    public virtual void Update()
    {
        if (_isUsed == false)
        {
            return;
        }

        if (_context.battleState.IsRunning() == false)
        {
            return;
        }

        _shotTimer += Time.deltaTime;

        if (_shotTimer >= _reloadTime)
        {
            _shotTimer = 0f;

            Shot();
        }
    }

    public virtual void Init(ActorBase actor, SkillDefinition data, SkillContext context)
    {
        _actor = actor;
        _context = context;

        transform.SetParent(_actor.transform, false);
        transform.localPosition = Vector3.zero;

        ID = data.id;
        NAME = data.name;
        _fireCount = data.fireCount;
        _multiShotCount = data.multiShotCount;
        _shotDelay = data.shotDelay;
        _reloadTime = data.reloadTime;
        _projectileData = data.projectileData;

        foreach (var item in _projectileData)
        {
            var role = ProjectileManager.instance.GetProjectileById(item.id);
            _context.actorSpawner.InitPool(role);
        }

        _isUsed = false;
    }

    public void SetUsed(bool isUsed)
    {
        _isUsed = isUsed;
        _shotTimer = 0f;
    }

    protected void Shot()
    {
        StartCoroutine(coShot());
    }

    protected virtual IEnumerator coShot()
    {
        for (int i = 0; i < _fireCount; i++)
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
