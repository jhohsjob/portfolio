using System;
using System.Collections.Generic;
using UnityEngine;


public class MercenaryViewPool : IDisposable
{
    private readonly GameObject _prefab;
    private readonly Transform _parent;

    private readonly Stack<MercenaryView> _pool = new();
    private readonly HashSet<MercenaryView> _inPool = new();

    public MercenaryViewPool(GameObject prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }

    public MercenaryView Get()
    {
        while (_pool.Count > 0)
        {
            var view = _pool.Pop();
            _inPool.Remove(view);

            if (view == null)
            {
                continue;
            }

            view.SetActive(true);
            return view;
        }

        return Create();
    }

    public void Release(MercenaryView view)
    {
        if (view == null || _inPool.Add(view) == false)
        {
            return;
        }

        view.ResetView();
        view.transform.SetParent(_parent, false);
        _pool.Push(view);
    }

    public void Dispose()
    {
        foreach (var view in _pool)
        {
            if (view != null)
            {
                UnityEngine.Object.Destroy(view.gameObject);
            }
        }
        _pool.Clear();
        _inPool.Clear();
    }

    private MercenaryView Create()
    {
        var obj = UnityEngine.Object.Instantiate(_prefab, _parent, false);
        var view = obj.AddComponent<MercenaryView>();
        view.Initialize();
        return view;
    }
}