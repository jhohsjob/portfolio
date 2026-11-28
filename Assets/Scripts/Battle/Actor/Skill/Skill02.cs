using System.Collections;
using UnityEngine;


public class Skill02 : Skill
{
    private ActorProjectile[] _slot;
    private float[] _slotAngles;

    public override void Init(ActorBase actor, SkillData data, IBattleEnvironment env)
    {
        base.Init(actor, data, env);

        _slot = new ActorProjectile[_fireCount];

        _slotAngles = new float[_fireCount];
        float angleStep = Mathf.PI * 2f / _fireCount;
        for (int i = 0; i < _fireCount; i++)
        {
            _slotAngles[i] = angleStep * i;
        }
    }

    public override void Update()
    {
        if (_isUsed == false)
        {
            return;
        }

        if (_env.state.IsRunning() == false)
        {
            return;
        }

        int emptySlot = System.Array.FindIndex(_slot, p => p == null);
        if (emptySlot < 0)
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

    protected override IEnumerator coShot()
    {
        int index = System.Array.FindIndex(_slot, p => p == null);
        if (index < 0)
        {
            yield break;
        }

        var role = _env.GetProjectile(_projectileData[0].Id);
        var position = _actor.muzzlePos;
        var projectile = _env.spawner.Spawn<ActorProjectile, ProjectileDefinition>(role, position);
        if (projectile.moveBehaviour is OrbitMove orbit)
        {
            orbit.Setup(_actor.transform, 1f, _slotAngles[index], 1f);
        }
        projectile.Shot(_actor);
        projectile.state.onStateChanged += (state) =>
        {
            if (state.HasFlag(ActorState.Die))
            {
                _slot[index] = null;
            }
        };

        _slot[index] = projectile;

        yield return new WaitForSeconds(_shotDelay);
    }
}