using UnityEngine;


[CreateAssetMenu(menuName = "GameDefinition/LoadPrefabs")]
public class LoadPrefabs : ScriptableObject
{
    public string containerName;
    public string path;
    public int loadCount;
}
