using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(MercenaryGradeTable))]
public class MercenaryGradeTableEditor : Editor
{
    private int _startGrade = 1;
    private int _endGrade = 5;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        _startGrade = EditorGUILayout.IntField("Start Grade", _startGrade);
        _endGrade = EditorGUILayout.IntField("End Grade", _endGrade);

        if (GUILayout.Button("Generate Grades"))
        {
            var table = (MercenaryGradeTable)target;

            table.GenerateGrades(_startGrade, _endGrade);

            EditorUtility.SetDirty(table);
            AssetDatabase.SaveAssets();
        }
    }
}