using UnityEngine;


public class ActorDIElement : Actor<DIElement, DIElementDefinition>, ICollectableDropItem
{
    [HideInInspector]
    public ElementType elementType => _role.elementType;

    public void OnCollectedByPlayer(Player player)
    {
        player.element.AddElement(this);

        Die();
    }
}
