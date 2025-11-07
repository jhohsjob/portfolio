public interface ICollectableDropItem
{
    void OnCollectedByPlayer(Player player);
}

public class ActorDropItem
{
    private ActorBase _actor;

    public DropItemType type { get; protected set; }

    public Body body;

    public void Init(ActorBase actor, DropItemType type)
    {
        this._actor = actor;
        this.type = type;
    }

    public void Die()
    {
        BattleManager.instance.actorManager.Return(_actor);
    }
}