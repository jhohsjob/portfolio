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

    private ComponentPool<HpBar> _pool;

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
        _pool?.Dispose();

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

            _pool = new ComponentPool<HpBar>(new ComponentPool<HpBar>.Options
            {
                name = "HpBar",
                prewarm = _expandCount,
                expandCount = _expandCount,
                create = () => Instantiate(_original, _poolContainer),
                onGet = bar =>
                {
                    bar.transform.SetParent(_activeContainer);
                    bar.gameObject.SetActive(true);
                },
                onRelease = bar =>
                {
                    bar.transform.SetParent(_poolContainer, false);
                    bar.transform.localPosition = Vector3.zero;
                    bar.gameObject.SetActive(false);
                }
            });

            _isLoadEnd = true;

            FlushWaitList();
        });
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

        var hpBar = _pool.Get();
        actor.SetHpBar(hpBar);
        _activeList[actor] = hpBar;
    }

    private void Disconnect(ActorView actor)
    {
        if (_activeList.TryGetValue(actor, out var hpBar) == false)
        {
            return;
        }

        _pool.Release(hpBar);
        _activeList.Remove(actor);
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
