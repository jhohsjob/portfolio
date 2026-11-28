using System;
using UnityEngine;


public class MercenaryViewPool : IDisposable
{
    private readonly ComponentPool<MercenaryView> _pool;

    public MercenaryViewPool(GameObject prefab, Transform parent)
    {
        _pool = new ComponentPool<MercenaryView>(new ComponentPool<MercenaryView>.Options
        {
            name = $"MercView:{prefab.name}",
            expandCount = 1,
            create = () =>
            {
                var view = UnityEngine.Object.Instantiate(prefab, parent, false).AddComponent<MercenaryView>();
                view.Initialize();
                return view;
            },
            onGet = view => view.SetActive(true),
            onRelease = view =>
            {
                view.ResetView();                      // 내부에서 SetActive(false)
                view.transform.SetParent(parent, false);
            }
        });
    }

    public MercenaryView Get() => _pool.Get();
    public void Release(MercenaryView view) => _pool.Release(view);
    public void Dispose() => _pool.Dispose();
}