using System.Collections.Generic;


public class MapLevel
{
    public int level;
    public int levelupCount;
    public int spawnCount;
    public float respawnTime;

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
        spawnData = data.spawnData;

        nextLevel = nextData == null ? -1 : nextData.level;

        spawnActorId.Clear();
        foreach (var info in spawnData.spawnInfos)
        {
            spawnActorId.Add(info.actorId);

            BattleManager.instance.actorManager.InitEnemy(info.actorId);

            foreach (var dropItemId in info.dropItemIds)
            {
                BattleManager.instance.actorManager.InitDropItem(dropItemId);
            }
        }

        spawnActorRate.Clear();
        spawnActorRate.AddRange(spawnData.spawnRate);
    }
}
