using UnityEngine;


public class Enemy : Actor
{
    public override Team team { get; protected set; } = Team.Enemy;

    private Vector3 _moveDirection = Vector3.zero;

    public override void Init(RoleData roleData)
    {
        var data = roleData as EnemyData;
        if (data == null)
        {
            return;
        }
        
        base.Init(roleData);
    }

    private void Update()
    {
        bool isControl = (_moveDirection != Vector3.zero && BattleManager.instance.battleStatus == BattleStatus.Run);
        if (isControl == true)
        {
            Move();
        }
    }

    public override void Enter()
    {
        base.Enter();

        transform.rotation = GetRotation(ref _moveDirection);
    }

    protected override void Move()
    {
        transform.rotation = GetRotation(ref _moveDirection);
        transform.Translate(Vector3.back * _moveSpeed * Time.deltaTime);
    }

    public override void Die()
    {
        _moveDirection = Vector3.zero;

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
