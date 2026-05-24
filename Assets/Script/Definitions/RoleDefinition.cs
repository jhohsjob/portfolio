#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;


public class RoleDefinition : ScriptableObject
{
    public RoleType roleType;
    public int id;
#if UNITY_EDITOR
    public MonoScript behaviourScript;
#endif
    [HideInInspector]
    public string behaviourTypeName;

    public GameObject body;
    public Vector3 resourceOffset = Vector3.zero;

    public float maxHP;
    public float moveSpeed;

    public ActorMoveType moveType;

    private Type _cachedBehaviourType;

    public Type behaviourType
    {
        get
        {
            if (_cachedBehaviourType != null)
            {
                return _cachedBehaviourType;
            }

            if (string.IsNullOrEmpty(behaviourTypeName))
            {
                return null;
            }

            _cachedBehaviourType = Type.GetType(behaviourTypeName);
            if (_cachedBehaviourType == null)
            {
                Debug.LogError($"Cannot load type from behaviourTypeName: {behaviourTypeName}");
            }

            return _cachedBehaviourType;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (behaviourScript != null)
        {
            var targetClass = behaviourScript.GetClass();
            if (targetClass != null)
            {
                behaviourTypeName = targetClass.AssemblyQualifiedName;
            }
            else
            {
                Debug.LogWarning($"[{name}] Cannot find class type from the specified script file. (Please check if the class name matches the file name.)");
                behaviourTypeName = null;
            }
        }
        else
        {
            behaviourTypeName = null;
        }

        _cachedBehaviourType = null;
    }
#endif

    private string LocalKey(string prefix) => $"{prefix}_{id}";

    public string GetNameKey() => LocalKey("name");
    public string GetDescKey() => LocalKey("desc");
}
