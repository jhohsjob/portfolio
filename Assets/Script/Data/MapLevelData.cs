using UnityEngine;


[CreateAssetMenu(menuName = "GameData/MapLevelData")]
public class MapLevelData : ScriptableObject
{
    public string mlName;
    public int level;
    public int levelupCount;
    public int spawnCount;
    public float respawnTime;
    public Vector3 growMapSize;
    public int growCameraSize;

    public SpawnData spawnData;
}
