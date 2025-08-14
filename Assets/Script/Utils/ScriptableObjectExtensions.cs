using UnityEngine;

public static class ScriptableObjectExtensions
{
    /// <summary>
    /// ScriptableObject�� ���� �����մϴ�.
    /// ���� �������� ��� �ʵ带 ������ �� �ν��Ͻ��� ��ȯ�մϴ�.
    /// </summary>
    public static T DeepCopy<T>(this T original) where T : ScriptableObject
    {
        if (original == null)
            return null;

        // �� ScriptableObject �ν��Ͻ� ����
        var copy = ScriptableObject.CreateInstance(original.GetType());

        // Unity�� JsonUtility�� �̿��� ��ü �ʵ� ����
        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(original), copy);

        return copy as T;
    }
}