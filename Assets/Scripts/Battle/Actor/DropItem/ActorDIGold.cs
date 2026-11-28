using UnityEngine;


public class ActorDIGold : Actor<DropItemGold>, ICollectableDropItem
{
    private float speed = 100f;
    private int _goldAmount;

    protected override void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime, Space.World);
    }

    public override void Init(DropItemGold role)
    {
        base.Init(role);

        _goldAmount = role.goldAmount;
    }

    public void OnCollectedByPlayer(Player player)
    {
        player.AddGold(_goldAmount);

        Die();
    }
}
