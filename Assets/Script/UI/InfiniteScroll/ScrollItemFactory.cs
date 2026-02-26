using UnityEngine;


public class MercenaryItemFactory : IScrollItemFactory
{
    private GameObject _prefab;

    public MercenaryItemFactory(GameObject prefab)
    {
        _prefab = prefab;
    }

    public GameObject CreateItem(Transform parent)
    {
        return GameObject.Instantiate(_prefab, parent);
    }
}

public class StageItemFactory : IScrollItemFactory
{
    private GameObject _prefab;

    public StageItemFactory(GameObject prefab)
    {
        _prefab = prefab;
    }

    public GameObject CreateItem(Transform parent)
    {
        return GameObject.Instantiate(_prefab, parent);
    }
}
