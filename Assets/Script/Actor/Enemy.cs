public class Enemy : Actor
{
    public override Enums.Team team { get; set; } = Enums.Team.Enemy;
    protected override float _moveSpeed { get; set; } = 3f;

    private void Awake()
    {
    }
    
    protected override void Move()
    {
    }

    public override void Die()
    {
        EnemyManager.instance.Die(this);
        SpawnManager.instance.Retrieve(Enums.SpawnType.Enemy, gameObject);
    }
}
