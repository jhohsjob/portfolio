using UnityEngine;


public class VerticalInfiniteScroll : InfiniteScrollBase
{
    [Header("column count")]
    [SerializeField]
    [Tooltip("0 = Auto")]
    protected int _fixedColumnCount = 0;

    private int _columnCount;
    private int _visibleRowCount;

    protected override bool IsVertical() => true;

    protected override void CalculateLayout()
    {
        if (_fixedColumnCount > 0)
        {
            _columnCount = _fixedColumnCount;
        }
        else
        {
            float width = _content.rect.width;
            float unit = _itemWidth + _spacingX;

            _columnCount = Mathf.Max(1, Mathf.FloorToInt((width + _spacingX) / unit));
        }
        
        _visibleRowCount = Mathf.CeilToInt(_scrollRect.viewport.rect.height / (_itemHeight + _spacingY)) + 2;
    }

    protected override int GetPoolSize()
    {
        return _columnCount * _visibleRowCount;
    }

    protected override void UpdateContentSize()
    {
        int rowCount = Mathf.CeilToInt((float)_itemCount / _columnCount);

        float height = rowCount * (_itemHeight + _spacingY);

        _content.sizeDelta = new Vector2(_content.sizeDelta.x, height);
    }

    protected override void UpdateItems()
    {
        float scrollY = _content.anchoredPosition.y;
        int firstRow = Mathf.FloorToInt(scrollY / (_itemHeight + _spacingY));

        float contentWidth = _content.rect.width;
        float gridWidth = _columnCount * _itemWidth + (_columnCount - 1) * _spacingX;

        float startX = Mathf.Max(0, (contentWidth - gridWidth) * 0.5f);

        for (int i = 0; i < _items.Count; i++)
        {
            int row = i / _columnCount;
            int col = i % _columnCount;

            int dataIndex = (firstRow + row) * _columnCount + col;

            var item = _items[i];

            if (dataIndex < 0 || dataIndex >= _itemCount)
            {
                item.gameObject.SetActive(false);
                continue;
            }

            item.gameObject.SetActive(true);

            RectTransform rt = item.GetComponent<RectTransform>();

            float x = startX + col * (_itemWidth + _spacingX);
            float y = -(firstRow + row) * (_itemHeight + _spacingY);
            rt.anchoredPosition = new Vector2(x, y);

            _provider.Bind(dataIndex, item);
        }
    }

    protected override float GetNearestSnapPosition()
    {
        float unit = _itemHeight + _spacingY;

        float viewportHeight = _scrollRect.viewport.rect.height;

        float currentY = _content.anchoredPosition.y;

        float centerInContent = currentY + viewportHeight * 0.5f;

        int index = Mathf.RoundToInt(centerInContent / unit);

        index = Mathf.Clamp(index, 0, _itemCount - 1);

        float itemCenter = index * unit + _itemHeight * 0.5f;

        float targetY = itemCenter - viewportHeight * 0.5f;

        return targetY;
    }

    protected override float GetSnapPositionByIndex(int index)
    {
        float unit = _itemHeight + _spacingY;
        float viewport = _scrollRect.viewport.rect.height;

        index = Mathf.Clamp(index, 0, _itemCount - 1);

        float itemCenter = index * unit + _itemHeight * 0.5f;

        return itemCenter - viewport * 0.5f;
    }
}