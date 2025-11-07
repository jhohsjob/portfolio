using UnityEngine;


public class Enemy : Actor<Monster, MonsterData>
{
    private Vector3 _moveDirection = Vector3.zero;

    private int _spawnMapLevel = 0;
    public int spawnMapLevel => _spawnMapLevel;

    private Vector3 _diePos = Vector3.zero;
    public Vector3 diePos => _diePos;

    private float _damage = 1f;

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

    public override void Init(Monster role)
    {
        base.Init(role);

        _body.cbTriggerEnter += OnBodyTriggerEnter;
    }

    public override void Enter(object data = null)
    {
        base.Enter(data);

        if (data is int)
        {
            _spawnMapLevel = (int)data;
        }

        UpdateDirection();
    }

    protected override void Move()
    {
        if (BattleManager.instance.debugEnemyPause)
            return;

        UpdateDirection();
        MoveTowardsPlayer(0.5f);
    }

    protected override void Die()
    {
        DieAfter();

        _moveDirection = Vector3.zero;
        _diePos = transform.localPosition;

        BattleManager.instance.enemyManager.Die(this);

        base.Die();
    }

    private void UpdateDirection()
    {
        var playerPos = BattleManager.instance.battleScene.player.transform.position;
        var dir = playerPos - transform.position;
        _moveDirection = dir.normalized;

        _body.FlipX(_moveDirection.x);
    }

    private void MoveTowardsPlayer(float stopDistance)
    {
        var playerPos = BattleManager.instance.battleScene.player.transform.position;
        var distance = Vector3.Distance(transform.position, playerPos);

        if (distance > stopDistance)
        {
            transform.Translate(_moveDirection * _role.moveSpeed * Time.deltaTime);
        }
    }

    private void OnBodyTriggerEnter(Body other)
    {
        if (other.TryGetComponent(out Body body))
        {
            if (body.actor is Player actor && actor.state.current != ActorState.Die)
            {
                actor.BeHit(_damage);
                ResetCollider();
            }
        }
    }
}
