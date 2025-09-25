using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameData/MapInfoData")]
public class MapInfoData : ScriptableObject
{
    public string mapName;
    public Transform map;
    public List<MapLevelData> levelDatas;
}
