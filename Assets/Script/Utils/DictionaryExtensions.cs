using System;
using System.Collections.Generic;
using System.Linq;

public static class DictionaryExtensions
{
    /// <summary>
    /// Dictionary�� �����մϴ�.
    /// ���� ���� Ÿ���� ���, cloneValueFunc�� ���� ���� ���� ����.
    /// </summary>
    /// <typeparam name="TKey">Dictionary Key Ÿ��</typeparam>
    /// <typeparam name="TValue">Dictionary Value Ÿ��</typeparam>
    /// <param name="source">���� Dictionary</param>
    /// <param name="cloneValueFunc">���� �����ϴ� �Լ� (null�̸� ���� ����)</param>
    /// <returns>���ο� Dictionary</returns>
    public static Dictionary<TKey, TValue> DeepCopy<TKey, TValue>(this Dictionary<TKey, TValue> source, Func<TValue, TValue> cloneValueFunc = null)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (cloneValueFunc == null)
        {
            // ���� ����
            return source.ToDictionary(entry => entry.Key, entry => entry.Value);
        }
        else
        {
            // ���� ����
            return source.ToDictionary(entry => entry.Key, entry => cloneValueFunc(entry.Value));
        }
    }
}