using System.Collections.Generic;
using UnityEngine;


public class DropItemActorManager : MonoBehaviour
{
    private struct DropInfo
    {
        public int roleId;
        public int rate;
    }

    private IActorSpawner _actorSpawner;

    private Dictionary<int, Dictionary<int, List<SpawnDefinition.DropItemEntry>>> _dropItemEntryDic = new();

    private List<ActorBase> _dropItemActorList = new ();

    public void Init(IActorSpawner _ctorSpawner)
    {
        _actorSpawner = _ctorSpawner;
    }

    public void SetSpawnList(MapLevel mapLevel)
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

    public void Spawn(int mapLevel, Enemy enemy)
    {
        var roleId = GetDropItemRoleId(mapLevel, enemy.roleId);
        var role = DropItemManager.instance.GetDropItemById(roleId);
        var position = enemy.diePos;

        ActorBase dropItem = null;
        switch (role)
        {
            case DIElement diElement:
                {
                    dropItem = _actorSpawner.Spawn<ActorDIElement, DIElementDefinition>(diElement, position);
                }
                break;

            case DIGold diGold:
                {
                    dropItem = _actorSpawner.Spawn<ActorDIGold, DIGoldDefinition>(diGold, position);
                }
                break;
        }

        if (dropItem != null)
        {
            _dropItemActorList.Add(dropItem);
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

    public void Return(ActorBase dropItem)
    {
        if (_dropItemActorList.Contains(dropItem) == false)
        {
            return;
        }

        _dropItemActorList.Remove(dropItem);
    }
}
