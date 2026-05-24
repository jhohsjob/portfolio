using System.Collections.Generic;
using UnityEngine;


public class HPBarController : MonoBehaviour
{
    private IAssetLoader _assetLoader;

    [SerializeField]
    private Camera _uiCamera;
    [SerializeField]
    private Transform _poolContianer;
    [SerializeField]
    private Transform _activeContianer;
    private RectTransform _rt;

    private HPBar _original;

    private Queue<HPBar> _pool = new();
    private Dictionary<int, HPBar> _activeList = new();

    private Queue<ActorBase> _waitList = new();

    private bool _isLoadEnd = false;

    private void Awake()
    {
        _isLoadEnd = false;
        
        _rt = GetComponent<RectTransform>();

        EventHelper.AddEventListener(EventName.HpBarConnection, OnHpBarConnection);
        EventHelper.AddEventListener(EventName.HpBarDisconnection, OnHpBarDisconnection);
    }

    private void OnDestroy()
    {
        EventHelper.RemoveEventListener(EventName.HpBarConnection, OnHpBarConnection);
        EventHelper.RemoveEventListener(EventName.HpBarDisconnection, OnHpBarDisconnection);
    }

    public void InitDependencies(IAssetLoader assetLoader)
    {
        _assetLoader = assetLoader;
    }

    public void Initialize()
    {
        _assetLoader.LoadPrefab("HPBar", (prefab) =>
        {
            _original = prefab.GetComponent<HPBar>();

            PoolGenerate();

            _isLoadEnd = true;

            FlushWaitList();
        });
    }

    private void PoolGenerate()
    {
        for (int i = 0; i < 10; i++)
        {
            var wait = Instantiate(_original, _poolContianer);
            wait.Init(_uiCamera, _rt);
            _pool.Enqueue(wait);
        }
    }

    private void FlushWaitList()
    {
        while (_waitList.Count > 0)
        {
            Connect(_waitList.Dequeue());
        }
    }

    private void Connect(ActorBase actor)
    {
        if (_pool.Count == 0)
        {
            PoolGenerate();
        }

        var hpBar = _pool.Dequeue();
        hpBar.transform.SetParent(_activeContianer, false);
        hpBar.SetTarget(actor);

        _activeList[actor.ID] = hpBar;
    }

    private void Disconnect(ActorBase actor)
    {
        if (_activeList.TryGetValue(actor.ID, out var hpBar) == false)
        {
            return;
        }

        hpBar.transform.SetParent(_poolContianer, false);
        hpBar.ResetTarget();

        _activeList.Remove(actor.ID);
        _pool.Enqueue(hpBar);
    }

    private void OnHpBarConnection(object sender, object data)
    {
        if (sender is not ActorBase actor)
        {
            return;
        }

        if (_isLoadEnd == false)
        {
            _waitList.Enqueue(actor);
            return;
        }

        Connect(actor);
    }

    private void OnHpBarDisconnection(object sender, object data)
    {
        if (sender is ActorBase actor)
        {
            Disconnect(actor);
        }
    }
}
