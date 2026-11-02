using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(IdSyncTable), true)]
public class IdSyncTableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Sync IDs"))
        {
            SyncIds();
        }
    }

    private void SyncIds()
    {
        SerializedProperty table = serializedObject.FindProperty("_table");

        if (table == null)
        {
            Debug.LogError(
                $"{target.GetType().Name} does not contain a '_table' field.");
            return;
        }

        SerializedProperty pairs = table.FindPropertyRelative("_pairs");

        if (pairs == null)
        {
            Debug.LogError(
                $"{target.GetType().Name} does not contain a '_pairs' field.");
            return;
        }

        Undo.RecordObject(target, "Sync IDs");

        for (int i = 0; i < pairs.arraySize; i++)
        {
            SerializedProperty pair =
                pairs.GetArrayElementAtIndex(i);

            SerializedProperty key =
                pair.FindPropertyRelative("_key");

            SerializedProperty value =
                pair.FindPropertyRelative("_value");

            var definition = value.objectReferenceValue as DefinitionBase;

            if (definition == null)
                continue;

            key.intValue = definition.id;

            EditorUtility.SetDirty(definition);
        }

        serializedObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(target);
    }
}