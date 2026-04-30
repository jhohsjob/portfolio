using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TMPFontChangerWindow : EditorWindow
{
    private TMP_FontAsset targetFont;
    private TMP_FontAsset onlyReplaceThis;

    [MenuItem("Tools/TMP/Font Changer")]
    public static void Open()
    {
        GetWindow<TMPFontChangerWindow>("TMP Font Changer");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);

        EditorGUILayout.LabelField("TMP Font Changer", EditorStyles.boldLabel);

        targetFont = (TMP_FontAsset)EditorGUILayout.ObjectField(
            "Target Font",
            targetFont,
            typeof(TMP_FontAsset),
            false
        );

        onlyReplaceThis = (TMP_FontAsset)EditorGUILayout.ObjectField(
            "Only Replace (Optional)",
            onlyReplaceThis,
            typeof(TMP_FontAsset),
            false
        );

        GUILayout.Space(10);

        GUI.enabled = targetFont != null;

        if (GUILayout.Button("Apply (Scene / Prefab)", GUILayout.Height(30)))
        {
            ApplyFont();
        }

        GUI.enabled = true;
    }

    private void ApplyFont()
    {
        int count = 0;

        // Prefab Mode 체크
        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

        if (prefabStage != null)
        {
            // 프리팹 수정 중
            var root = prefabStage.prefabContentsRoot;

            count += ApplyToRoot(root);

            EditorSceneManager.MarkSceneDirty(root.scene);

            Debug.Log($"[Prefab] TMP 폰트 변경 완료: {count}개");
        }
        else
        {
            // 일반 씬
            var scene = SceneManager.GetActiveScene();
            var roots = scene.GetRootGameObjects();

            foreach (var root in roots)
            {
                count += ApplyToRoot(root);
            }

            EditorSceneManager.MarkSceneDirty(scene);

            Debug.Log($"[Scene] TMP 폰트 변경 완료: {count}개");
        }
    }

    private int ApplyToRoot(GameObject root)
    {
        int count = 0;

        var texts = root.GetComponentsInChildren<TextMeshProUGUI>(true);

        foreach (var t in texts)
        {
            if (onlyReplaceThis != null && t.font != onlyReplaceThis)
            {
                continue;
            }

            Undo.RecordObject(t, "Change TMP Font");
            t.font = targetFont;
            EditorUtility.SetDirty(t);
            count++;
        }

        return count;
    }
}