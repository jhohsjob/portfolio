using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    private struct InstantiateData
    {
        public GameObject prefab;
        public GameObject container;

        public InstantiateData(GameObject prefab, GameObject container)
        {
            this.prefab = prefab;
            this.container = container;
        }
    }

    [SerializeField]
    public GameObject _enemyContainer;

    [SerializeField]
    public GameObject _projectileContainer;

    private GameObject _enemyPrefab;
    private GameObject _projectilePrefab;

    private Dictionary<Enums.SpawnType, Queue<GameObject>> _spawnWaitList = new Dictionary<Enums.SpawnType, Queue<GameObject>>();
    private Dictionary<Enums.SpawnType, InstantiateData> _instantiateData = new Dictionary<Enums.SpawnType, InstantiateData>();

    protected override void OnAwake()
    {
        _enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");
        _projectilePrefab = Resources.Load<GameObject>("Prefabs/Projectile");

        _spawnWaitList.Add(Enums.SpawnType.Enemy, new Queue<GameObject>());
        _spawnWaitList.Add(Enums.SpawnType.Projectile, new Queue<GameObject>());

        _instantiateData.Add(Enums.SpawnType.Enemy, new InstantiateData(_enemyPrefab, _enemyContainer));
        _instantiateData.Add(Enums.SpawnType.Projectile, new InstantiateData(_projectilePrefab, _projectileContainer));
    }

    public void Init()
    {
        MakeWait(Enums.SpawnType.Enemy, 10);
        MakeWait(Enums.SpawnType.Projectile, 10);
    }

    public void MakeWait(Enums.SpawnType spawnType, int count)
    {
        var prefab = _instantiateData[spawnType].prefab;
        var container = _instantiateData[spawnType].container;

        for (int i = 0; i < count; i++)
        {
            var wait = Instantiate(prefab, container.transform);
            _spawnWaitList[spawnType].Enqueue(wait);
        }
    }

    public GameObject Spawn(Enums.SpawnType spawnType, int count, Transform parent)
    {
        if (_spawnWaitList[spawnType].Count < count)
        {
            MakeWait(spawnType, count);
        }

        var spawn = _spawnWaitList[spawnType].Dequeue();
        spawn.transform.SetParent(parent);
        spawn.transform.localPosition = Vector3.zero;

        return spawn;
    }

    public void Retrieve(Enums.SpawnType spawnType, GameObject obj)
    {
        if (_spawnWaitList[spawnType].Contains(obj) == true)
        {
            //error

            return;
        }

        _spawnWaitList[spawnType].Enqueue(obj);

        obj.transform.SetParent(_enemyContainer.transform);
        obj.transform.localPosition = Vector3.zero;
    }
}