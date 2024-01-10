using UnityEngine;


public class DIElement : DropItem
{
    [HideInInspector]
    public ElementType elementType;

    public override void Init(RoleData roleData)
    {
        var data = roleData as DIElementData;
        if (data == null)
        {
            return;
        }

        elementType = data.elementType;

        base.Init(roleData);
    }

    public override void Enter(object data = null)
    {
        base.Enter(data);

        var diElementData = _roleData as DIElementData;
    }

}
