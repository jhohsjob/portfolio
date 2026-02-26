using System.Collections.Generic;
using UnityEngine;


public class DropItemActorManager : MonoBehaviour
{
    private struct DropInfo
    {
        public int roleId;
        public int rate;
    }

    private BattleManager _gameManager = null;

    private Dictionary<int, Dictionary<int, List<SpawnDefinition.DropItemEntry>>> _dropItemEntryDic = new();

    private List<ActorBase> _dropItemActorList = new ();

    private int _dieCount = 0;
    public int dieCount => _dieCount;

    public void Init(BattleManager gameManager)
    {
        _gameManager = gameManager;
    }

    public void SetDropList(MapLevel mapLevel)
    {
        if (_dropItemEntryDic.ContainsKey(mapLevel.level))
        {
            return;
        }

        var dropItemEntryDic = new Dictionary<int, List<SpawnDefinition.DropItemEntry>>();

        foreach (var enemyEntry in mapLevel.spawnDefinition.enemyEntries)
        {
            dropItemEntryDic[enemyEntry.roleId] = enemyEntry.dropItemEntries;
        }

        _dropItemEntryDic[mapLevel.level] = dropItemEntryDic;
    }

    public void Spawn(Enemy enemy)
    {
        var roleId = GetDropItemRoleId(enemy.spawnMapLevel, enemy.roleId);
        var role = DropItemManager.instance.GetDropItemById(roleId);
        var parent = _gameManager.battleScene.actorContainer;
        var position = enemy.diePos;
        ActorBase dropItem = null;
        switch (role)
        {
            case DIElement diElement:
                dropItem = _gameManager.actorManager.GetActor(diElement, parent, position);
                (dropItem as ActorDIElement).Enter();
                break;

            case DIGold diGold:
                dropItem = _gameManager.actorManager.GetActor(diGold, parent, position);
                (dropItem as ActorDIGold).Enter();
                break;
        }

        if (dropItem != null)
        {
            _dropItemActorList.Add(dropItem);

            EventHelper.Send(EventName.EnemySpawnEnd, this, dropItem);
        }
    }

    public int GetDropItemRoleId(int mapLevel, int roleId)
    {
        if (_dropItemEntryDic.TryGetValue(mapLevel, out var enemyEntries) == false)
        {
            return -1;
        }

        if (enemyEntries.TryGetValue(roleId, out var dropItemEntries) == false)
        {
            return -1;
        }

        int totalRate = 0;
        foreach (var dropItemEntry in dropItemEntries)
        {
            totalRate += dropItemEntry.spawnRate;
        }

        var rnd = Random.Range(0, totalRate);
        var id = 0;

        for (int i = 0; i < dropItemEntries.Count; i++)
        {
            if (dropItemEntries[i].spawnRate > rnd)
            {
                id = dropItemEntries[i].roleId;
                break;
            }

            rnd -= dropItemEntries[i].spawnRate;
        }

        return id;
    }

    public void Die(ActorBase dropItem)
    {
        if (_dropItemActorList.Contains(dropItem) == false)
        {
            return;
        }

        _dropItemActorList.Remove(dropItem);

        _gameManager.actorManager.Return(dropItem);

        _dieCount++;

        // EventHelper.Send(EventName.EnemyDieEnd, this, dropItem);
    }
}
