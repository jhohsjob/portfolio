using System;
using System.Collections.Generic;
using System.Linq;

public static class DictionaryExtensions
{
    /// <summary>
    /// Dictionary를 복사합니다.
    /// 값이 참조 타입일 경우, cloneValueFunc을 통해 깊은 복사 가능.
    /// </summary>
    /// <typeparam name="TKey">Dictionary Key 타입</typeparam>
    /// <typeparam name="TValue">Dictionary Value 타입</typeparam>
    /// <param name="source">원본 Dictionary</param>
    /// <param name="cloneValueFunc">값을 복제하는 함수 (null이면 얕은 복사)</param>
    /// <returns>새로운 Dictionary</returns>
    public static Dictionary<TKey, TValue> DeepCopy<TKey, TValue>(this Dictionary<TKey, TValue> source, Func<TValue, TValue> cloneValueFunc = null)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (cloneValueFunc == null)
        {
            // 얕은 복사
            return source.ToDictionary(entry => entry.Key, entry => entry.Value);
        }
        else
        {
            // 깊은 복사
            return source.ToDictionary(entry => entry.Key, entry => cloneValueFunc(entry.Value));
        }
    }
}