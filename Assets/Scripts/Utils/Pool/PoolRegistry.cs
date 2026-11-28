using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public static class PoolRegistry
{
    private static readonly List<IPoolStats> _pools = new();

    public static IReadOnlyList<IPoolStats> pools => _pools;

    [Conditional("DEBUG_MODE")]
    public static void Register(IPoolStats pool) => _pools.Add(pool);

    [Conditional("DEBUG_MODE")]
    public static void Unregister(IPoolStats pool) => _pools.Remove(pool);

    // Domain Reload 비활성화 대비
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset() => _pools.Clear();
}