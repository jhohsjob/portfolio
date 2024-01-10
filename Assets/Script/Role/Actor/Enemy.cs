using UnityEngine;


public class Enemy : Actor
{
    public override Team team { get; protected set; } = Team.Enemy;

    private Vector3 _moveDirection = Vector3.zero;

    private int _spawnMapLevel = 0;
    public int spawnMapLevel => _spawnMapLevel;

    private Vector3 _diePos = Vector3.zero;
    public Vector3 diePos => _diePos;

    private void Update()
    {
        bool isControl = (_moveDirection != Vector3.zero && BattleManager.instance.battleStatus == BattleStatus.Run);
        if (isControl == true)
        {
            Move();
        }
    }

    public override void Init(RoleData roleData)
    {
        var data = roleData as EnemyData;
        if (data == null)
        {
            return;
        }
        
        base.Init(roleData);
    }

    public override void Enter(object data = null)
    {
        base.Enter(data);

        if (data is int)
        {
            _spawnMapLevel = (int)data;
        }

        transform.rotation = GetRotation(ref _moveDirection);
    }

    protected override void Move()
    {
        transform.rotation = GetRotation(ref _moveDirection);
        transform.Translate(Vector3.back * _moveSpeed * Time.deltaTime);
    }

    protected override void Die()
    {
        _moveDirection = Vector3.zero;
        _diePos = transform.localPosition;

        BattleManager.instance.enemyManager.Die(this);

        base.Die();
    }

    private Quaternion GetRotation(ref Vector3 direction)
    {
        var playerPos = BattleManager.instance.battleScene.player.transform.position;
        var dir = transform.position - playerPos;
        dir.y = 0f;
        direction = dir.normalized;
        return Quaternion.LookRotation(dir);
    }
}
