#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


public class RoleData : ScriptableObject
{
    public int id;
    public string roleName;
    public string roleDescription;
#if UNITY_EDITOR
    public MonoScript behaviourScript;
#endif
    [HideInInspector]
    public string behaviourTypeName;

    public string resourcePath;
    public GameObject body;
    public Vector3 resourceOffset = Vector3.zero;
    public float moveSpeed;

    public RoleType roleType;

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
