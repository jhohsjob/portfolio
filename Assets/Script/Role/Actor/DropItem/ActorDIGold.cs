using UnityEngine;


public class ActorDIGold : Actor<DIGold, DIGoldData>, ICollectableDropItem
{
    private ActorDropItem _dropItem = new();

    private float speed = 100f;

    void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime, Space.World);
    }

    public override void Init(DIGold role)
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
        player.AddGold(_role.gold);

        Die();
    }
}
