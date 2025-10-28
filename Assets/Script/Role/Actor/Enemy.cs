using UnityEngine;


public class Enemy : Actor<Monster, MonsterData>
{
    private Vector3 _moveDirection = Vector3.zero;

    private int _spawnMapLevel = 0;
    public int spawnMapLevel => _spawnMapLevel;

    private Vector3 _diePos = Vector3.zero;
    public Vector3 diePos => _diePos;

    protected override void Awake()
    {
        base.Awake();

        team = Team.Enemy;
    }

    private void Update()
    {
        bool isControl = _moveDirection != Vector3.zero && BattleManager.instance.IsBattleRun();
        if (isControl == true)
        {
            Move();
        }
    }

    public override void Enter(object data = null)
    {
        base.Enter(data);

        _hp.Init(_role.maxHP);

        if (data is int)
        {
            _spawnMapLevel = (int)data;
        }

        SetRotation(ref _moveDirection);
    }

    protected override void Move()
    {
        if (BattleManager.instance.debugEnemyPause == true)
        {
            return;
        }

        SetRotation(ref _moveDirection);
        transform.Translate(_moveDirection * _role.moveSpeed * Time.deltaTime);
    }

    protected override void Die()
    {
        _moveDirection = Vector3.zero;
        _diePos = transform.localPosition;

        BattleManager.instance.enemyManager.Die(this);

        base.Die();
    }

    private void SetRotation(ref Vector3 direction)
    {
        var playerPos = BattleManager.instance.battleScene.player.transform.position;
        var dir = playerPos - transform.position;
        direction = dir.normalized;

        _body.FlipX(_moveDirection.x);
    }
}
