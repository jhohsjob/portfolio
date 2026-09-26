using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(MercenaryLevelTable))]
public class MercenaryLevelTableEditor : Editor
{
    private int _startLevel = 1;
    private int _endLevel = 50;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        _startLevel = EditorGUILayout.IntField("Start Level", _startLevel);
        _endLevel = EditorGUILayout.IntField("End Level", _endLevel);

        if (GUILayout.Button("Generate Levels"))
        {
            var table = (MercenaryLevelTable)target;

            table.GenerateLevels(_startLevel, _endLevel);

            EditorUtility.SetDirty(table);
            AssetDatabase.SaveAssets();
        }
    }
}