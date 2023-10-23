using UnityEngine;


[CreateAssetMenu(menuName = "GameData/LoadPrefabs")]
public class LoadPrefabs : ScriptableObject
{
    public string containerName;
    public string path;
    public int loadCount;
}
