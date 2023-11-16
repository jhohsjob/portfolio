using System.Collections.Generic;
using UnityEngine;


public class HPBarController : MonoBehaviour
{
    [SerializeField]
    private Transform _poolContianer;
    [SerializeField]
    private Transform _activeContianer;
    [SerializeField]
    private Actor _debugActor = null;

    private Queue<HPBar> _pool = new Queue<HPBar>();
    private Dictionary<int, HPBar> _activeList = new Dictionary<int, HPBar>();

    private HPBar _hpBarPrefab;

    private Vector3 LOAD_POSITION = new Vector3(1000f, 1000f, 1000f);

    private void Awake()
    {
        EventHelper.AddEventListener(EventName.EnemySpawnEnd, OnEnemySpawnEnd);
        EventHelper.AddEventListener(EventName.EnemyDieEnd, OnEnemyDieEnd);

        _hpBarPrefab = Resources.Load<HPBar>("UI/HPBar");

        PoolGenerate();
    }

    private void PoolGenerate()
    {
        for (int i = 0; i < 10; i++)
        {
            var wait = Instantiate(_hpBarPrefab, _poolContianer);
            wait.Init();
            _pool.Enqueue(wait);
        }
    }

    private void OnEnemySpawnEnd(object sender, object data)
    {
        if (data is Enemy enemy == false)
        {
            return;
        }

        if (_pool.Count == 0)
        {
            PoolGenerate();
        }

        var hpBar = _pool.Dequeue();
        hpBar.transform.SetParent(_activeContianer.transform, false);
        hpBar.transform.localPosition = LOAD_POSITION;
        hpBar.SetTarget(enemy);

        _debugActor = enemy;

        _activeList.Add(enemy.ID, hpBar);
    }

    private void OnEnemyDieEnd(object sender, object data)
    {
        if (data is Enemy enemy == false)
        {
            return;
        }

        var hpBar = _activeList[enemy.ID];
        hpBar.transform.SetParent(_poolContianer.transform, false);
        hpBar.ResetTarget();

        _debugActor = null;

        _activeList.Remove(enemy.ID);
        _pool.Enqueue(hpBar);
    }
}
