using UnityEngine;


public class ActorDIElement : Actor<DropItemElement>, ICollectableDropItem
{
    [HideInInspector]
    public ElementType elementType;

    public override void Init(RoleBase role)
    {
        base.Init(role);

        if (role is not DropItemElement diElement)
        {
            Debug.LogError("Invalid role type for ActorDIElement");
            return;
        }

        elementType = diElement.elementType;
    }

    public void OnCollectedByPlayer(Player player)
    {
        player.element.AddElement(this);

        Die();
    }
}