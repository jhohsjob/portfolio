using System.Collections.Generic;
using UnityEngine;

public static class WaitCache
{
    private static readonly Dictionary<float, WaitForSeconds> _cache = new();

    public static WaitForSeconds Get(float seconds)
    {
        if (_cache.TryGetValue(seconds, out var wait) == false)
        {
            wait = new WaitForSeconds(seconds);
            _cache[seconds] = wait;
        }
        return wait;
    }
}
