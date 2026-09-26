using UnityEngine;


public class ActorDIGold : Actor<DIGold>, ICollectableDropItem
{
    private float speed = 100f;
    private int _goldAmount;

    protected override void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime, Space.World);
    }

    public override void Init(RoleBase role)
    {
        base.Init(role);

        if (role is not DIGold gold)
        {
            return;
        }

        _goldAmount = gold.goldAmount;
    }

    public void OnCollectedByPlayer(Player player)
    {
        player.AddGold(_goldAmount);

        Die();
    }
}
