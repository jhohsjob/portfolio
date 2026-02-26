using System.Collections.Generic;


public class MapLevel
{
    public int level;
    public int levelupCount;
    public int spawnCount;
    public float respawnTime;

    public SpawnDefinition spawnDefinition;

    public List<SpawnRuntimeInfo> spawnInfos = new();

    public int nextLevel;

    public MapLevel(MapLevelDefinition data, MapLevelDefinition nextData)
    {
        level = data.level;
        levelupCount = data.levelupCount;
        spawnCount = data.spawnCount;
        respawnTime = data.respawnTime;
        spawnDefinition = data.spawnDefinition;

        nextLevel = nextData == null ? -1 : nextData.level;

        BuildRuntimeSpawnInfos();
        PreloadActors();
    }

    private void BuildRuntimeSpawnInfos()
    {
        spawnInfos.Clear();

        if (spawnDefinition == null)
        {
            return;
        }

        foreach (var enemyEntry in spawnDefinition.enemyEntries)
        {
            var runtimeInfo = new SpawnRuntimeInfo(enemyEntry);
            spawnInfos.Add(runtimeInfo);
        }
    }

    private void PreloadActors()
    {
        var actorManager = BattleManager.instance.actorManager;

        foreach (var spawnInfo in spawnInfos)
        {
            actorManager.InitEnemy(spawnInfo.roleId);

            foreach (var dropItemRoleId in spawnInfo.dropItemRoleIds)
            {
                actorManager.InitDropItem(dropItemRoleId);
            }
        }
    }
}
