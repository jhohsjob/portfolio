using System.Collections.Generic;


public class SpawnRuntimeInfo
{
    public int roleId;
    public int spawnRate;

    public List<int> dropItemRoleIds = new();
    public List<int> dropItemRates = new();

    public int dropRateSum;

    public SpawnRuntimeInfo(SpawnDefinition.EnemyEntry source)
    {
        roleId = source.roleId;
        spawnRate = source.spawnRate;

        dropRateSum = 0;

        if (source.dropItemEntries != null)
        {
            foreach (var item in source.dropItemEntries)
            {
                dropItemRoleIds.Add(item.roleId);
                dropItemRates.Add(item.spawnRate);

                dropRateSum += item.spawnRate;
            }
        }
    }
}