using UnityEngine;


public class RoleData : ScriptableObject
{
    public int id;
    public string roleName;

    public string resourcePath;
    public Vector3 resourceOffset = Vector3.zero;
    public float moveSpeed;
}
