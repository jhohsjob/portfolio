using UnityEngine;


[CreateAssetMenu(menuName = "GameData/GameLevelData")]
public class GameLevelData : ScriptableObject
{
    public int level;
    public int needSpawnCount;
    public Vector3 growMapSize;
    public int growCameraSize;
}
