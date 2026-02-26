using UnityEngine;


[CreateAssetMenu(menuName = "GameDefinition/MapLevelDefinition")]
public class MapLevelDefinition : ScriptableObject
{
    public string mlName;
    public int level;
    public int levelupCount;
    public int spawnCount;
    public float respawnTime;

    public SpawnDefinition spawnDefinition;
}
