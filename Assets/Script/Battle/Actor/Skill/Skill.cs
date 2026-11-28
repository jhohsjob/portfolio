using System.Collections;
using UnityEngine;


public class Skill : MonoBehaviour
{
    protected IBattleEnvironment _env;

    protected ActorBase _actor;

    public int Id { get; private set; }
    public string Name { get; private set; }

    protected int _fireCount { get; private set; }
    protected int _multiShotCount { get; private set; }
    protected float _shotDelay { get; private set; }
    protected float _reloadTime { get; private set; }
    protected float _spreadAngle { get; private set; }
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

        if (_env.state.IsRunning() == false)
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

    public virtual void Init(ActorBase actor, SkillData data, IBattleEnvironment env)
    {
        _actor = actor;
        _env = env;

        transform.SetParent(_actor.transform, false);
        transform.localPosition = Vector3.zero;

        Id = data.id;
        Name = data.name;
        _fireCount = data.fireCount;
        _multiShotCount = data.multiShotCount;
        _spreadAngle = data.spreadAngle;
        _shotDelay = data.shotDelay;
        _reloadTime = data.reloadTime;
        _projectileData = data.projectileData;

        foreach (var item in _projectileData)
        {
            var role = _env.GetProjectile(item.Id);
            _env.spawner.InitPool(role);
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
            var role = _env.GetProjectile(_projectileData[0].Id);
            var position = _actor.muzzlePos;
            var projectile = _env.spawner.Spawn<ActorProjectile, ProjectileDefinition>(role, position);
            projectile.Shot(_actor);

            yield return new WaitForSeconds(_shotDelay);
        }

        yield return null;
    }
}
