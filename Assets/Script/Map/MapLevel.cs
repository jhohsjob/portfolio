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
    /// <summary>
    /// key : actor id, value : rate
    /// </summary>
    public Dictionary<int, int> spawnDatas;

    public int nextLevel;

    public MapLevel(MapLevelData data, MapLevelData nextData)
    {
        level = data.level;
        levelupCount = data.levelupCount;
        spawnCount = data.spawnCount;
        respawnTime = data.respawnTime;
        growMapSize = data.growMapSize;
        growCameraSize = data.growCameraSize;
        spawnDatas = data.spawnDatas;

        nextLevel = nextData == null ? -1 : nextData.level;

        foreach (var actorId in spawnDatas.Keys)
        {
            GameManager.instance.roleManager.InitEnemy(actorId);
        }
    }

    public int GetSpawnActorId()
    {
        var rnd = Random.Range(0, 100);
        var id = 0;

        foreach (var pair in spawnDatas)
        {
            if (pair.Value > rnd)
            {
                id = pair.Key;
                break;
            }

            rnd -= pair.Value;
        }

        return id;
    }
}
