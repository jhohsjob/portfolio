using UnityEngine;


public interface IScrollDataProvider
{
    int GetItemCount();
    void Bind(int index, GameObject item);
}

public interface IScrollItemFactory
{
    GameObject CreateItem(Transform parent);
}