using System.Collections.Generic;


public class EnemyManager : MonoSingleton<EnemyManager>
{
    private GameScene _gameScene;

    private List<Enemy> _enemyList = new List<Enemy>();

    private int _dieCount = 0;
    public int dieCount => _dieCount;

    protected override void OnAwake()
    {
        
    }

    public void Init(GameScene gameScene)
    {
        _gameScene = gameScene;
    }

    public void Spawn(Enemy enemy)
    {
        _enemyList.Add(enemy);
    }

    public void Die(Enemy enemy)
    {
        if (_enemyList.Contains(enemy) == false)
        {
            return;
        }

        _enemyList.Remove(enemy);

        enemy.Die();

        _dieCount++;
    }
}
