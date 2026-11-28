using UnityEngine;


public class ActorDIElement : Actor<DropItemElement>, ICollectableDropItem
{
    [HideInInspector]
    public ElementType elementType;

    public override void Init(DropItemElement role)
    {
        base.Init(role);

        elementType = role.elementType;
    }

    public void OnCollectedByPlayer(Player player)
    {
        player.element.AddElement(this);

        Die();
    }
}