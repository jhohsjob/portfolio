using UnityEditor;
using UnityEngine.UI;


[CustomEditor(typeof(UIButton))]
public class UIButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var uiButton = (UIButton)target;

        var button = uiButton.GetComponent<Button>();

        SerializedProperty property = serializedObject.FindProperty("_button");
        if (property != null)
        {
            property.objectReferenceValue = button;
        }
        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }
}