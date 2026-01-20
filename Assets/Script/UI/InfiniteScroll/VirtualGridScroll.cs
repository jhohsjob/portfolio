using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VirtualGridScroll : MonoBehaviour
{
    [SerializeField]
    private ScrollRect _scrollRect;
    [SerializeField]
    private RectTransform _content;

    [SerializeField]
    private float _itemWidth = 150f;
    [SerializeField]
    private float _itemHeight = 200f;
    [SerializeField]
    private float _spacingX = 20f;
    [SerializeField]
    private float _spacingY = 20f;

    private IScrollDataProvider _provider;
    private IScrollItemFactory _itemFactory;
    private readonly List<GameObject> _pool = new();

    private int _columnCount;
    private int _visibleRowCount;
    private int _itemCount;

    private void OnEnable()
    {
        Canvas.willRenderCanvases += OnCanvasDirty;
    }

    private void OnDisable()
    {
        Canvas.willRenderCanvases -= OnCanvasDirty;
    }

    public void Initialize(IScrollDataProvider provider, IScrollItemFactory factory, int itemCount)
    {
        _provider = provider;
        _itemFactory = factory;
        _itemCount = itemCount;

        Build();
    }

    private void Build()
    {
        _columnCount = CalculateColumnCount();

        BuildPool();
        UpdateContentSize();
        UpdateItems();

        _scrollRect.onValueChanged.RemoveAllListeners();
        _scrollRect.onValueChanged.AddListener(_ => UpdateItems());
    }

    private int CalculateColumnCount()
    {
        float width = _content.rect.width;
        float unit = _itemWidth + _spacingX;

        int count = Mathf.FloorToInt((width + _spacingX) / unit);

        return Mathf.Max(1, count);
    }

    private void BuildPool()
    {
        foreach (var obj in _pool)
        {
            Destroy(obj);
        }
        _pool.Clear();

        _visibleRowCount = Mathf.CeilToInt(_scrollRect.viewport.rect.height / (_itemHeight + _spacingY)) + 2;

        int poolSize = _visibleRowCount * _columnCount;

        for (int i = 0; i < poolSize; i++)
        {
            _pool.Add(_itemFactory.CreateItem(_content));
        }
    }

    private void UpdateContentSize()
    {
        int rowCount = Mathf.CeilToInt((float)_itemCount / _columnCount);

        float height = rowCount * (_itemHeight + _spacingY);

        _content.sizeDelta = new Vector2(_content.sizeDelta.x, height);
    }

    private void UpdateItems()
    {
        float scrollY = _content.anchoredPosition.y;

        int firstRow = Mathf.FloorToInt(scrollY / (_itemHeight + _spacingY));

        float contentWidth = _content.rect.width;

        float gridWidth = _columnCount * _itemWidth + (_columnCount - 1) * _spacingX;

        float startX = Mathf.Max(0, (contentWidth - gridWidth) * 0.5f);

        for (int i = 0; i < _pool.Count; i++)
        {
            int row = i / _columnCount;
            int col = i % _columnCount;

            int dataIndex = (firstRow + row) * _columnCount + col;

            var item = _pool[i];

            if (dataIndex < 0 || dataIndex >= _itemCount)
            {
                item.SetActive(false);
                continue;
            }

            item.SetActive(true);

            RectTransform rt = item.GetComponent<RectTransform>();

            float x = startX + col * (_itemWidth + _spacingX);

            float y = -(firstRow + row) * (_itemHeight + _spacingY);

            rt.anchoredPosition = new Vector2(x, y);

            _provider.Bind(dataIndex, item);
        }
    }

    private void OnCanvasDirty()
    {
        int newColumn = CalculateColumnCount();
        if (newColumn != _columnCount)
        {
            _columnCount = newColumn;
            Build();
        }
    }
}