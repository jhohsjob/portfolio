using System.Collections.Generic;
using UnityEngine;


public class HpBarController : MonoBehaviour
{
    private IAssetService _assetService;

    private Camera _mainCamera;
    [SerializeField]
    private Camera _uiCamera;
    [SerializeField]
    private Transform _poolContainer;
    [SerializeField]
    private Transform _activeContainer;
    [SerializeField]
    private RectTransform _rt;
    [SerializeField]
    private int _expandCount = 10;

    private HpBar _original;

    private Queue<HpBar> _pool = new();
    private Dictionary<ActorView, HpBar> _activeList = new();

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
        foreach (var pair in _activeList)
        {
            var screenPos = _mainCamera.WorldToScreenPoint(pair.Key.transform.position);
            screenPos.z = 0f;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rt, screenPos, _uiCamera, out var localPos);

            pair.Value.FollowPosition(localPos);
        }
    }

    public void InitDependencies(Camera mainCamera, IAssetService assetService)
    {
        _mainCamera = mainCamera;
        _assetService = assetService;
    }

    public void Initialize()
    {
        _assetService.LoadPrefab("HpBar", (prefab) =>
        {
            _original = prefab.GetComponent<HpBar>();

            ExpandPool();

            _isLoadEnd = true;

            FlushWaitList();
        });
    }

    private void ExpandPool()
    {
        for (int i = 0; i < _expandCount; i++)
        {
            var wait = Instantiate(_original, _poolContainer);
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

        hpBar.transform.SetParent(_activeContainer);
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

        hpBar.transform.SetParent(_poolContainer, false);
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
