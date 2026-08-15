using System.Collections.Generic;
using UnityEngine;


public class MercenaryViewPool
{
    private readonly Transform _parent;
    private readonly GameObject _original;

    private readonly Queue<MercenaryView> _pool = new();

    public MercenaryViewPool(GameObject original, Transform parent)
    {
        _original = original;
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
        
        {
            var obj = Object.Instantiate(_original, _parent);
            var view = obj.AddComponent<MercenaryView>();
            view.Initialize();
            return view;
        }
    }

    public void Release(MercenaryView view)
    {
        view.ResetView();
        _pool.Enqueue(view);
    }

    public void OnDestroy()
    {
        while (_pool.Count > 0)
        {
            var view = _pool.Dequeue();
            Object.Destroy(view.gameObject);
        }
    }
}