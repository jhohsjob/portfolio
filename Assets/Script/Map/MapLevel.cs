using System.Collections.Generic;
using UnityEngine;


public class MapLevel
{
    public int level;
    public int levelupCount;
    public int spawnCount;
    public float respawnTime;
    public Vector3 growMapSize;
    public int growCameraSize;

    public SpawnData spawnData;

    public List<int> spawnActorId = new List<int>();
    public List<int> spawnActorRate = new List<int>();

    public int nextLevel;

    public MapLevel(MapLevelData data, MapLevelData nextData)
    {
        level = data.level;
        levelupCount = data.levelupCount;
        spawnCount = data.spawnCount;
        respawnTime = data.respawnTime;
        growMapSize = data.growMapSize;
        growCameraSize = data.growCameraSize;
        spawnData = data.spawnData;

        nextLevel = nextData == null ? -1 : nextData.level;

        spawnActorId.Clear();
        foreach (var info in spawnData.spawnInfos)
        {
            spawnActorId.Add(info.actorId);

            BattleManager.instance.roleManager.InitEnemy(info.actorId);

            foreach (var dropItemId in info.dropItemIds)
            {
                BattleManager.instance.roleManager.InitDropItem(dropItemId);
            }
        }

        spawnActorRate.Clear();
        spawnActorRate.AddRange(spawnData.spawnRate);
    }
}
