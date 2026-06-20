using System.Collections.Generic;
using UnityEngine;


public class HPBarManager : MonoBehaviour
{
    private IAssetLoader _assetLoader;

    private Camera _mainCamera;
    [SerializeField]
    private Camera _uiCamera;
    [SerializeField]
    private Transform _poolContianer;
    [SerializeField]
    private Transform _activeContianer;
    [SerializeField]
    private RectTransform _rt;
    [SerializeField]
    private int _expandCount = 10;

    private HPBar _original;

    private Queue<HPBar> _pool = new();
    private Dictionary<ActorView, HPBar> _activeList = new();

    private Queue<ActorView> _waitList = new();

    private bool _isLoadEnd = false;

    private void Awake()
    {
        _isLoadEnd = false;
        
        EventHelper.AddEventListener(EventName.HpBarConnection, OnHpBarConnection);
        EventHelper.AddEventListener(EventName.HpBarDisconnection, OnHpBarDisconnection);
    }

    private void OnDestroy()
    {
        EventHelper.RemoveEventListener(EventName.HpBarConnection, OnHpBarConnection);
        EventHelper.RemoveEventListener(EventName.HpBarDisconnection, OnHpBarDisconnection);
    }

    private void LateUpdate()
    {
        foreach (var actor in _activeList.Keys)
        {
            if (_activeList.TryGetValue(actor, out var hpBar) == false)
            {
                continue;
            }

            var screenPos = _mainCamera.WorldToScreenPoint(actor.transform.position);
            screenPos.z = 0f;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rt, screenPos, _uiCamera, out var localPos);

            _activeList[actor].FollowPosition(localPos);
        }
    }

    public void InitDependencies(Camera mainCamera, IAssetLoader assetLoader)
    {
        _mainCamera = mainCamera;
        _assetLoader = assetLoader;
    }

    public void Initialize()
    {
        _assetLoader.LoadPrefab("HPBar", (prefab) =>
        {
            _original = prefab.GetComponent<HPBar>();

            ExpandPool();

            _isLoadEnd = true;

            FlushWaitList();
        });
    }

    private void ExpandPool()
    {
        for (int i = 0; i < _expandCount; i++)
        {
            var wait = Instantiate(_original, _poolContianer);
            wait.transform.localPosition = Vector3.zero;
            _pool.Enqueue(wait);
        }
    }

    private void FlushWaitList()
    {
        while (_waitList.Count > 0)
        {
            var actor = _waitList.Dequeue();
            if (actor == null)
            {
                continue;
            }

            Connect(actor);
        }
    }

    private void Connect(ActorView actor)
    {
        if (_activeList.ContainsKey(actor))
        {
            return;
        }

        if (_pool.Count == 0)
        {
            ExpandPool();
        }

        var hpBar = _pool.Dequeue();

        hpBar.transform.SetParent(_activeContianer);
        hpBar.gameObject.SetActive(true);

        actor.SetHpBar(hpBar);

        _activeList[actor] = hpBar;
    }

    private void Disconnect(ActorView actor)
    {
        if (_activeList.TryGetValue(actor, out var hpBar) == false)
        {
            return;
        }

        hpBar.transform.SetParent(_poolContianer, false);
        hpBar.transform.localPosition = Vector3.zero;
        hpBar.gameObject.SetActive(false);

        _activeList.Remove(actor);

        _pool.Enqueue(hpBar);
    }

    private void OnHpBarConnection(object sender, object data)
    {
        if (sender is not ActorView actor)
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
        if (sender is ActorView actor)
        {
            Disconnect(actor);
        }
    }
}
