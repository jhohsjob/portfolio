using SerializableDictionary.Scripts;
using System.Collections.Generic;
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

    [Tooltip("key : actor id, value : rate")]
    [SerializeField]
    private SerializableDictionary<int, int> _spawnDatas;
    public Dictionary<int, int> spawnDatas => _spawnDatas.Dictionary;
}
