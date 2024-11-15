using System.Collections.Generic;
using UnityEngine;


public class DropItemManager : MonoBehaviour
{
    private struct DropInfo
    {
        public int roleId;
        public int rate;
    }

    private BattleManager _gameManager = null;

    private Dictionary<int, SpawnData> _spawnDatas = new Dictionary<int, SpawnData>();

    private List<DropItem> _dropItemList = new List<DropItem>();

    private int _dieCount = 0;
    public int dieCount => _dieCount;

    public void Init(BattleManager gameManager)
    {
        _gameManager = gameManager;
    }

    public void SetDropList(MapLevel mapLevel)
    {
        _spawnDatas.Add(mapLevel.level, mapLevel.spawnData);
    }

    public void Spawn(Enemy enemy)
    {
        var data = GameTable.GetDropItemData(GetDropItemId(enemy.spawnMapLevel, enemy.roleId));
        var parent = _gameManager.battleScene.actorContainer;
        var position = enemy.diePos;
        var dropItem = _gameManager.roleManager.GetRole(data, parent, position) as DropItem;
        dropItem.Enter();

        _dropItemList.Add(dropItem);

        EventHelper.Send(EventName.EnemySpawnEnd, this, dropItem);
    }

    public int GetDropItemId(int mapLevel, int roleId)
    {
        var rnd = Random.Range(0, 100);
        var id = 0;

        var spawnInfos = _spawnDatas[mapLevel].spawnInfos;

        foreach (var spawnInfo in spawnInfos)
        {
            if (spawnInfo.actorId == roleId)
            {
                for (int i = 0; i < spawnInfo.dropItemIds.Count; i++)
                {
                    if (spawnInfo.dropItemRate.Count <= i)
                    {
                        id = spawnInfo.dropItemIds[i];
                        break;
                    }

                    if (spawnInfo.dropItemRate[i] > rnd)
                    {
                        id = spawnInfo.dropItemIds[i];
                        break;
                    }

                    rnd -= spawnInfo.dropItemRate[i];
                }

                break;
            }
        }

        return id;
    }

    public void Die(DropItem dropItem)
    {
        if (_dropItemList.Contains(dropItem) == false)
        {
            return;
        }

        _dropItemList.Remove(dropItem);

        _gameManager.roleManager.Retrieve(dropItem);

        _dieCount++;

        // EventHelper.Send(EventName.EnemyDieEnd, this, dropItem);
    }
}
