using UnityEngine;

public static class ScriptableObjectExtensions
{
    /// <summary>
    /// ScriptableObject를 깊은 복사합니다.
    /// 원본 데이터의 모든 필드를 복사한 새 인스턴스를 반환합니다.
    /// </summary>
    public static T DeepCopy<T>(this T original) where T : ScriptableObject
    {
        if (original == null)
            return null;

        // 새 ScriptableObject 인스턴스 생성
        T copy = ScriptableObject.CreateInstance<T>();

        // Unity의 JsonUtility를 이용해 전체 필드 복사
        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(original), copy);

        return copy;
    }
}