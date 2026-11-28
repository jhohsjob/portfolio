using System;
using System.Collections.Generic;
using UnityEngine;


public interface IPoolStats
{
    string name { get; }
    int countAll { get; }
    int countActive { get; }
    int countInactive { get; }
    int peakActive { get; }
}

public sealed class ComponentPool<T> : IPoolStats, IDisposable where T : Component
{
    public sealed class Options
    {
        public string name = typeof(T).Name;
        public int prewarm = 0;
        public int expandCount = 10;
        /// <summary> 0 = 제한 없음. 초과분은 반환 시 Destroy </summary>
        public int maxInactive = 0;

        public Func<T> create;
        public Action<T> onGet;
        /// <summary> 생성 직후에도 호출된다 (= 대기 상태로 만드는 책임) </summary>
        public Action<T> onRelease;
        public Action<T> onDestroy;
    }


    private readonly Options _options;
    private readonly Stack<T> _inactive = new();
#if UNITY_EDITOR || DEBUG_MODE
    private readonly HashSet<T> _inactiveSet = new();   // 이중 반환 감지용
#endif

    public string name => _options.name;
    public int countAll { get; private set; }
    public int countInactive => _inactive.Count;
    public int countActive => countAll - _inactive.Count;
    public int peakActive { get; private set; }

    public ComponentPool(Options options)
    {
        if (options.create == null)
        {
            throw new ArgumentException("create is required");
        }

        _options = options;

        if (_options.prewarm > 0)
        {
            Expand(_options.prewarm);
        }

        PoolRegistry.Register(this);
    }

    public T Get()
    {
        T item = null;

        while (item == null)
        {
            if (_inactive.Count == 0)
            {
                Expand(_options.expandCount);
            }

            item = _inactive.Pop();
#if UNITY_EDITOR || DEBUG_MODE
            _inactiveSet.Remove(item);
#endif
            if (item == null)
            {
                countAll--;   // 외부에서 파괴된 오브젝트
            }
        }

        _options.onGet?.Invoke(item);

        if (countActive > peakActive)
        {
            peakActive = countActive;
        }

        return item;
    }

    public void Release(T item)
    {
        if (item == null)
        {
            return;
        }

#if UNITY_EDITOR || DEBUG_MODE
        if (_inactiveSet.Add(item) == false)
        {
            Debug.LogError($"[Pool:{name}] Double release detected: {item.name}");
            return;
        }
#endif
        _options.onRelease?.Invoke(item);

        if (_options.maxInactive > 0 && _inactive.Count >= _options.maxInactive)
        {
#if UNITY_EDITOR || DEBUG_MODE
            _inactiveSet.Remove(item);
#endif
            DestroyItem(item);
            countAll--;
            return;
        }

        _inactive.Push(item);
    }

    public void Dispose()
    {
        PoolRegistry.Unregister(this);

        while (_inactive.Count > 0)
        {
            var item = _inactive.Pop();
            if (item != null)
            {
                DestroyItem(item);
            }
        }

        countAll = 0;
#if UNITY_EDITOR || DEBUG_MODE
        _inactiveSet.Clear();
#endif
    }

    private void Expand(int count)
    {
        count = Mathf.Max(1, count);

        for (int i = 0; i < count; i++)
        {
            var item = _options.create();
            countAll++;

            _options.onRelease?.Invoke(item);   // 대기 상태로 정규화
            _inactive.Push(item);
#if UNITY_EDITOR || DEBUG_MODE
            _inactiveSet.Add(item);
#endif
        }
    }

    private void DestroyItem(T item)
    {
        if (_options.onDestroy != null)
        {
            _options.onDestroy(item);
        }
        else
        {
            UnityEngine.Object.Destroy(item.gameObject);
        }
    }
}