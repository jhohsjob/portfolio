using UnityEngine;


public class ActorDIGold : Actor<DIGold, DIGoldDefinition>, ICollectableDropItem
{
    private float speed = 100f;

    protected override void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime, Space.World);
    }

    public void OnCollectedByPlayer(Player player)
    {
        player.AddGold(_role.gold);

        Die();
    }
}
