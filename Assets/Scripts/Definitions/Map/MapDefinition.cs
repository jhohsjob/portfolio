using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameDefinition/MapDefinition")]
public class MapDefinition : ScriptableObject
{
    public string mapName;
    public Transform map;
    public List<MapLevelDefinition> levelDefinitions;
}
