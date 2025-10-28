using System.Collections;
using UnityEngine;


public class Skill02 : Skill
{
    private ActorProjectile[] _slot;
    private float[] _slotAngles;

    public override void Init(ActorBase actor, SkillData data)
    {
        base.Init(actor, data);

        _slot = new ActorProjectile[_shotCount];

        _slotAngles = new float[_shotCount];
        float angleStep = Mathf.PI * 2f / _shotCount;
        for (int i = 0; i < _shotCount; i++)
        {
            _slotAngles[i] = angleStep * i;
        }
    }

    public override void Update()
    {
        if (BattleManager.instance.IsBattleRun() == false)
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

        var moveData = new OrbitMoveData
        {
            radius = 1f,
            startAngle = _slotAngles[index],
            angularSpeed = 1f
        };

        var role = ProjectileHander.instance.GetProjectileById(_projectileData[0].id);
        var parent = BattleManager.instance.battleScene.actorContainer;
        var position = _actor.point.GetChild(0).transform.position;
        var projectile = BattleManager.instance.actorManager.GetActor(role, parent, position) as ActorProjectile;
        projectile.Shot(_actor, moveData);
        projectile.state.OnStateChanged += (state) =>
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