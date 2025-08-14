using UnityEngine;


public class ActorDIElement : Actor<DIElement, DIElementData>, ICollectableDropItem
{
    private ActorDropItem _dropItem = new();

    [HideInInspector]
    public ElementType elementType => _role.elementType;


    public override void Init(DIElement role)
    {
        base.Init(role);

        _dropItem.Init(this, _role.type);
    }

    protected override void Die()
    {
        _dropItem.Die();

        base.Die();
    }

    public void OnCollectedByPlayer(Player player)
    {
        player.AddElement(this);

        Die();
    }
}
