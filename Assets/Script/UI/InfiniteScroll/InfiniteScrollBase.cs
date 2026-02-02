using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public abstract class InfiniteScrollBase : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    protected ScrollRect _scrollRect;
    [SerializeField]
    protected RectTransform _content;

    [SerializeField]
    protected float _itemWidth = 150f;
    [SerializeField]
    protected float _itemHeight = 200f;
    [SerializeField]
    protected float _spacingX = 20f;
    [SerializeField]
    protected float _spacingY = 20f;

    [Header("Snap")]
    [SerializeField]
    protected bool _enableSnap = false;
    [SerializeField]
    protected float _snapSpeed = 10f;

    [Header("Center Scale")]
    [SerializeField]
    protected bool _useCenterScale = false;
    [SerializeField]
    protected float _maxScale = 1.2f;
    [SerializeField]
    protected float _minScale = 0.85f;
    [SerializeField]
    protected float _scaleRange = 300f;

    private bool _initializeEnd;

    protected IScrollDataProvider _provider;
    protected IScrollItemFactory _itemFactory;

    protected readonly List<InfiniteScrollItem> _items = new List<InfiniteScrollItem>();
    protected int _itemCount;

    private bool _isSnapping;
    private float _targetPos;
    private bool _isDragging;
    private bool _snapCompleted;
    private Vector2 _lastContentPos;
    private const float MOVE_EPSILON = 0.01f;

    protected ScrollOptions _options;
    public ScrollOptions options => _options;

    private void OnDisable()
    {
        _scrollRect.onValueChanged.RemoveAllListeners();
    }

    protected virtual void Update()
    {
        if (_initializeEnd == false)
        {
            return;
        }

        bool moved = HasMoved();
        if (_useCenterScale && (moved || _isSnapping))
        {
            UpdateItemScale();
        }

        if (_isDragging == true)
        {
            CachePos();
            return;
        }

        if (_enableSnap == true)
        {
            HandleSnap();
        }

        CachePos();
    }
    
    #region Update

    private bool HasMoved()
    {
        return Vector2.Distance(_content.anchoredPosition, _lastContentPos) > MOVE_EPSILON;
    }

    private void CachePos()
    {
        _lastContentPos = _content.anchoredPosition;
    }

    private void HandleSnap()
    {
        if (_isSnapping == false && _snapCompleted == false)
        {
            StartSnap();
        }

        if (_isSnapping)
        {
            UpdateSnap();
        }
    }

    #endregion

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragging = true;
        _isSnapping = false;
        _snapCompleted = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;
    }

    public void Initialize(IScrollDataProvider provider, IScrollItemFactory factory, int itemCount, int initPos = 0)
    {
        _initializeEnd = false;

        _provider = provider;
        _itemFactory = factory;
        _itemCount = itemCount;

        _options = new ScrollOptions(_useCenterScale);

        Build();
        if (_useCenterScale)
        {
            SetInitialPosition(initPos);
        }

        _initializeEnd = true;
    }

    protected void Build()
    {
        CalculateLayout();
        BuildItem();
        UpdateContentSize();
        UpdateItems();

        _scrollRect.onValueChanged.RemoveAllListeners();
        _scrollRect.onValueChanged.AddListener(_ =>
        {
            UpdateItems();
        });
    }

    protected void BuildItem()
    {
        foreach (var obj in _items)
        {
            Destroy(obj.gameObject);
        }
        _items.Clear();

        int poolSize = GetPoolSize();

        for (int i = 0; i < poolSize; i++)
        {
            var go = _itemFactory.CreateItem(_content);

            var item = go.GetComponent<InfiniteScrollItem>();
            if (item == null)
            {
                continue;
            }

            item.Init(this);
            _items.Add(item);

            go.SetActive(false);
        }
    }

    #region snap

    protected void StartSnap()
    {
        if (_snapCompleted == true)
        {
            return;
        }

        _isSnapping = true;
        _snapCompleted = true;

        _scrollRect.StopMovement();
        _scrollRect.velocity = Vector2.zero;

        _targetPos = GetNearestSnapPosition();
    }

    protected void UpdateSnap()
    {
        Vector2 pos = _content.anchoredPosition;
        float current = IsVertical() ? pos.y : pos.x;
        float next = Mathf.MoveTowards(current, _targetPos, _snapSpeed * Time.deltaTime);
        if (IsVertical())
        {
            pos.y = next;
        }
        else
        {
            pos.x = next;
        }
        _content.anchoredPosition = pos;

        if (Mathf.Abs(next - _targetPos) < 0.01f)
        {
            FinishSnap();
        }
    }

    protected void FinishSnap()
    {
        Vector2 pos = _content.anchoredPosition;
        if (IsVertical())
        {
            pos.y = _targetPos;
        }
        else
        {
            pos.x = _targetPos;
        }
        _content.anchoredPosition = pos;

        _isSnapping = false;

        UpdateItemScale();
    }

    #endregion

    #region scale

    protected void UpdateItemScale()
    {
        foreach (var item in _items)
        {
            if (item.gameObject.activeSelf == false)
            {
                continue;
            }

            RectTransform rt = item.transform as RectTransform;

            Vector3 viewPos = GetItemCenterInView(rt);

            float scale = CalculateScaleByViewPos(viewPos);

            item.SetScale(scale);
        }
    }

    public float CalculateScaleForItem(InfiniteScrollItem item)
    {
        if (item.gameObject.activeSelf == false)
        {
            return 1f;
        }

        RectTransform rt = item.transform as RectTransform;

        Vector3 viewPos = GetItemCenterInView(rt);

        float scale = CalculateScaleByViewPos(viewPos);

        return scale;
    }

    private Vector3 GetItemCenterInView(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        Vector3 center = (corners[0] + corners[2]) * 0.5f;

        return _scrollRect.viewport.InverseTransformPoint(center);
    }

    private float CalculateScaleByViewPos(Vector3 viewPos)
    {
        RectTransform vp = _scrollRect.viewport;
        float center = IsVertical() ? (0.5f - vp.pivot.y) * vp.rect.height : (0.5f - vp.pivot.x) * vp.rect.width;
        float deadZone = IsVertical() ? _itemHeight * 0.2f : _itemWidth * 0.2f;

        float pos = IsVertical() ? viewPos.y : viewPos.x;
        float dist = Mathf.Abs(pos - center);

        if (dist < deadZone)
        {
            return _maxScale;
        }

        float t = Mathf.Clamp01(dist / (_scaleRange * 0.8f));
        t = Mathf.Pow(t, 1.5f);

        return Mathf.Lerp(_maxScale, _minScale, t);
    }

    #endregion

    #region index & position

    protected void SetInitialPosition(int index)
    {
        float target = GetSnapPositionByIndex(index);

        Vector2 pos = _content.anchoredPosition;

        if (IsVertical())
        {
            pos.y = target;
        }
        else
        {
            pos.x = target;
        }

        _content.anchoredPosition = pos;
    }

    public void MoveToIndex(int index, bool smooth = true)
    {
        float target = GetSnapPositionByIndex(index);

        if (smooth)
        {
            _targetPos = target;
            _isSnapping = true;
            _snapCompleted = true;

            _scrollRect.StopMovement();
            _scrollRect.velocity = Vector2.zero;
        }
        else
        {
            Vector2 pos = _content.anchoredPosition;

            if (IsVertical())
            {
                pos.y = target;
            }
            else
            {
                pos.x = target;
            }

            _content.anchoredPosition = pos;
        }
    }

    public int GetCenteredIndex()
    {
        if (_items.Count == 0)
        {
            return -1;
        }

        float viewportCenter = GetViewportCenterInContent();

        float minDist = float.MaxValue;
        int result = -1;

        for (int i = 0; i < _items.Count; i++)
        {
            var item = _items[i];

            if (item.gameObject.activeSelf == false)
            {
                continue;
            }

            RectTransform rt = item.transform as RectTransform;

            float itemPos = IsVertical() ? rt.anchoredPosition.y : rt.anchoredPosition.x;

            float dist = Mathf.Abs(itemPos - viewportCenter);

            if (dist < minDist)
            {
                minDist = dist;
                result = item.index;
            }
        }

        return result;
    }

    private float GetViewportCenterInContent()
    {
        RectTransform vp = _scrollRect.viewport;

        if (IsVertical())
        {
            return -_content.anchoredPosition.y + (0.5f - vp.pivot.y) * vp.rect.height;
        }
        else
        {
            return -_content.anchoredPosition.x + (0.5f - vp.pivot.x) * vp.rect.width;
        }
    }

    #endregion

    protected abstract bool IsVertical();

    protected abstract void CalculateLayout();
    protected abstract int GetPoolSize();
    protected abstract void UpdateContentSize();
    protected abstract void UpdateItems();

    protected abstract float GetNearestSnapPosition();
    protected abstract float GetSnapPositionByIndex(int index);
}