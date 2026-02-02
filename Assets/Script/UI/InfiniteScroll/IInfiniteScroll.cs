using UnityEngine;


public interface IScrollDataProvider
{
    int GetItemCount();
    void Bind(int index, InfiniteScrollItem item);
}

public interface IScrollItemFactory
{
    GameObject CreateItem(Transform parent);
}