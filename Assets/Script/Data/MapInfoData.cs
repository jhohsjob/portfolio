using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameData/MapInfoData")]
public class MapInfoData : ScriptableObject
{
    public string mapName;
    public List<MapLevelData> levelDatas;

}
