using System.Collections.Generic;
using UnityEngine;

public class MercenaryViewPool
{
    private readonly Transform _parent;
    private readonly GameObject _prefab;

    private readonly Queue<MercenaryView> _pool = new();

    public MercenaryViewPool(GameObject prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }

    public MercenaryView Get()
    {
        if (_pool.Count > 0)
        {
            var view = _pool.Dequeue();
            view.gameObject.SetActive(true);
            return view;
        }

        var obj = Object.Instantiate(_prefab, _parent);
        return obj.AddComponent<MercenaryView>();
    }

    public void Release(MercenaryView view)
    {
        view.ResetView();
        _pool.Enqueue(view);
    }
}