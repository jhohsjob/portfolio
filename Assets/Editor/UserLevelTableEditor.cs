using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(UserLevelTable))]
public class UserLevelTableEditor : Editor
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
            var table = (UserLevelTable)target;

            table.GenerateLevels(_startLevel, _endLevel);

            EditorUtility.SetDirty(table);
            AssetDatabase.SaveAssets();
        }
    }
}