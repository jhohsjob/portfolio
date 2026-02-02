using UnityEngine;


public class HorizontalInfiniteScroll : InfiniteScrollBase
{
    [Header("row count")]
    [SerializeField]
    [Tooltip("0 = Auto")]
    protected int _fixedRowCount = 0;

    private int _rowCount;
    private int _visibleColumnCount;

    protected override bool IsVertical() => false;

    protected override void CalculateLayout()
    {
        if (_fixedRowCount > 0)
        {
            _rowCount = _fixedRowCount;
        }
        else
        {
            float height = _scrollRect.viewport.rect.height;
            float unit = _itemHeight + _spacingY;

            _rowCount = Mathf.Max(1, Mathf.FloorToInt((height + _spacingY) / unit));
        }

        _visibleColumnCount = Mathf.CeilToInt(_scrollRect.viewport.rect.width / (_itemWidth + _spacingX)) + 2;
    }

    protected override int GetPoolSize()
    {
        return _rowCount * _visibleColumnCount;
    }

    protected override void UpdateContentSize()
    {
        int columnCount = Mathf.CeilToInt((float)_itemCount / _rowCount);

        float width = columnCount * (_itemWidth + _spacingX);

        _content.sizeDelta = new Vector2(width, _content.sizeDelta.y);
    }

    protected override void UpdateItems()
    {
        float scrollX = _content.anchoredPosition.x;
        int firstColumn = Mathf.FloorToInt(-scrollX / (_itemWidth + _spacingX));

        float contentHeight = _content.rect.height;
        float gridHeight = _rowCount * _itemHeight + (_rowCount - 1) * _spacingY;

        float startY = Mathf.Max(0, (contentHeight - gridHeight) * 0.5f);

        for (int i = 0; i < _items.Count; i++)
        {
            int col = i / _rowCount;
            int row = i % _rowCount;

            int dataIndex = (firstColumn + col) * _rowCount + row;

            var item = _items[i];

            if (dataIndex < 0 || dataIndex >= _itemCount)
            {
                item.gameObject.SetActive(false);
                continue;
            }

            item.gameObject.SetActive(true);

            RectTransform rt = item.GetComponent<RectTransform>();

            float x = (firstColumn + col) * (_itemWidth + _spacingX);
            float y = -(startY + row * (_itemHeight + _spacingY));
            rt.anchoredPosition = new Vector2(x, y);

            _provider.Bind(dataIndex, item);
        }
    }

    protected override float GetNearestSnapPosition()
    {
        float unit = _itemWidth + _spacingX;

        float viewportWidth = _scrollRect.viewport.rect.width;

        float currentX = _content.anchoredPosition.x;

        float centerInContent = -currentX + viewportWidth * 0.5f;

        int index = Mathf.RoundToInt(centerInContent / unit);

        index = Mathf.Clamp(index, 0, _itemCount - 1);

        float itemCenter = index * unit + _itemWidth * 0.5f;

        float targetX = -(itemCenter - viewportWidth * 0.5f);

        return targetX;
    }

    protected override float GetSnapPositionByIndex(int index)
    {
        float unit = _itemWidth + _spacingX;
        float viewport = _scrollRect.viewport.rect.width;

        index = Mathf.Clamp(index, 0, _itemCount - 1);

        float itemCenter = index * unit + _itemWidth * 0.5f;

        return -(itemCenter - viewport * 0.5f);
    }
}