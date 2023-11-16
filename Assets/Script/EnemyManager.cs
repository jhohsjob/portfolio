using System.Collections.Generic;
using UnityEngine;


public class EnemyManager : MonoBehaviour
{
    private GameManager _gameManager = null;

    private List<Enemy> _enemyList = new List<Enemy>();

    private int _dieCount = 0;
    public int dieCount => _dieCount;

    public void Init(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public void Spawn(int id)
    {
        var data = GameTable.GetEnemyData(id);
        var parent = _gameManager.gameScene.actorContainer;
        var position = _gameManager.gameScene.GetRandomPos();
        var enemy = _gameManager.roleManager.GetRole(data, parent, position) as Enemy;
        enemy.Enter();

        _enemyList.Add(enemy);

        EventHelper.Send(EventName.EnemySpawnEnd, this, enemy);
    }

    public void Die(Enemy enemy)
    {
        if (_enemyList.Contains(enemy) == false)
        {
            return;
        }

        _enemyList.Remove(enemy);

        _gameManager.roleManager.Retrieve(enemy);

        _dieCount++;

        EventHelper.Send(EventName.EnemyDieEnd, this, enemy);
    }
}
