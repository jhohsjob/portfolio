#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


[CreateAssetMenu(menuName = "GameData/SkillData")]
public class SkillData : ScriptableObject
{
    public int id;
    public string skillName;
#if UNITY_EDITOR
    public MonoScript behaviourScript;
#endif
    [HideInInspector]
    public string behaviourTypeName;

    public int projectileId;

    public int shotCount;
    public float shotDelay;
    public float reloadTime;

    public System.Type behaviourType
    {
        get
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(behaviourTypeName))
            {
                behaviourTypeName = behaviourScript != null ? behaviourScript.GetClass().AssemblyQualifiedName : null;
            }
#endif

            if (string.IsNullOrEmpty(behaviourTypeName))
            {
                return null;
            }

            var type = System.Type.GetType(behaviourTypeName);
            if (type == null)
            {
                Debug.LogError($"Cannot load type from behaviourTypeName: {behaviourTypeName}");
            }

            return type;
        }
    }
}
