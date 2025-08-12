using UnityEngine;


public class RoleData : ScriptableObject
{
    public int id;
    public string roleName;
    public string roleDescription;

    public string resourcePath;
    public Vector3 resourceOffset = Vector3.zero;
    public float moveSpeed;

    public virtual void Init(RoleData data)
    {
        id = data.id;
        roleName = data.roleName;
        roleDescription = data.roleDescription;
        resourcePath = data.resourcePath;
        resourceOffset = data.resourceOffset;
        moveSpeed = data.moveSpeed;
    }
}
