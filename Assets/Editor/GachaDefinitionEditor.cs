#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GachaDefinition))]
public class GachaDefinitionEditor : Editor
{
    private SerializedProperty _id;
    private SerializedProperty _purchaseType;
    private SerializedProperty _currencyType;
    private SerializedProperty _price;
    private SerializedProperty _drawCount;
    private SerializedProperty _rewards;
    private SerializedProperty _scrollUI;
    private SerializedProperty _mainUI;

    private void OnEnable()
    {
        _id = serializedObject.FindProperty("_id");
        _purchaseType = serializedObject.FindProperty("purchaseType");
        _currencyType = serializedObject.FindProperty("currencyType");
        _price = serializedObject.FindProperty("price");
        _drawCount = serializedObject.FindProperty("drawCount");
        _rewards = serializedObject.FindProperty("rewards");
        _scrollUI = serializedObject.FindProperty("scrollUI");
        _mainUI = serializedObject.FindProperty("mainUI");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawBasicSettings();

        EditorGUILayout.Space(10);

        DrawRewardSettings();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawBasicSettings()
    {
        EditorGUILayout.LabelField("Gacha", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(_id);
        EditorGUILayout.PropertyField(_purchaseType);
        EditorGUILayout.PropertyField(_currencyType);
        EditorGUILayout.PropertyField(_price);
        EditorGUILayout.PropertyField(_drawCount);
        EditorGUILayout.PropertyField(_scrollUI);
        EditorGUILayout.PropertyField(_mainUI);
    }

    private void DrawRewardSettings()
    {
        EditorGUILayout.LabelField("Rewards", EditorStyles.boldLabel);

        if (_rewards == null)
        {
            EditorGUILayout.HelpBox("Rewards property was not found.", MessageType.Error);

            return;
        }

        EditorGUILayout.PropertyField(_rewards, true);

        EditorGUILayout.Space(5);

        if (_rewards.arraySize == 0)
        {
            EditorGUILayout.HelpBox("Add at least one reward.", MessageType.Info);

            return;
        }

        DrawRewardSummary();

        EditorGUILayout.Space(5);

        if (GUILayout.Button("Auto Assign Probability"))
        {
            AutoAssignProbability();
        }
    }

    private void DrawRewardSummary()
    {
        float totalWeight = 0f;
        float totalProbability = 0f;

        for (int i = 0; i < _rewards.arraySize; i++)
        {
            SerializedProperty reward = _rewards.GetArrayElementAtIndex(i);

            SerializedProperty weight = reward.FindPropertyRelative("weight");

            SerializedProperty probability = reward.FindPropertyRelative("probability");

            totalWeight += weight.floatValue;
            totalProbability += probability.floatValue;
        }

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.LabelField("Total Weight", totalWeight.ToString("0.####"));

        EditorGUILayout.LabelField("Total Probability", $"{totalProbability:0.####}%");

        MessageType messageType;

        if (Mathf.Approximately(totalProbability, 100f))
        {
            messageType = MessageType.Info;
        }
        else
        {
            messageType = MessageType.Warning;
        }

        EditorGUILayout.HelpBox($"Probability Total: {totalProbability:0.####}%", messageType);

        EditorGUILayout.EndVertical();
    }

    private void AutoAssignProbability()
    {
        if (_rewards.arraySize == 0)
        {
            return;
        }

        float totalWeight = 0f;

        for (int i = 0; i < _rewards.arraySize; i++)
        {
            SerializedProperty reward = _rewards.GetArrayElementAtIndex(i);

            SerializedProperty weight = reward.FindPropertyRelative("weight");

            totalWeight += Mathf.Max(0f, weight.floatValue);
        }

        if (totalWeight <= 0f)
        {
            EditorUtility.DisplayDialog("Invalid Weight", "Total weight must be greater than 0.", "OK");

            return;
        }

        Undo.RecordObject(target, "Auto Assign Gacha Probability");

        for (int i = 0; i < _rewards.arraySize; i++)
        {
            SerializedProperty reward = _rewards.GetArrayElementAtIndex(i);

            SerializedProperty weight = reward.FindPropertyRelative("weight");

            SerializedProperty probability = reward.FindPropertyRelative("probability");

            float validWeight = Mathf.Max(0f, weight.floatValue);

            probability.floatValue = validWeight / totalWeight * 100f;
        }

        serializedObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(target);
    }
}

#endif