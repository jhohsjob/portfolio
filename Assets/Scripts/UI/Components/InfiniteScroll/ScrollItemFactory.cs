using UnityEngine;


public class ScrollItemFactory : IScrollItemFactory
{
    private GameObject _prefab;

    public ScrollItemFactory(GameObject prefab)
    {
        _prefab = prefab;
    }

    public GameObject CreateItem(Transform parent)
    {
        return GameObject.Instantiate(_prefab, parent);
    }
}
