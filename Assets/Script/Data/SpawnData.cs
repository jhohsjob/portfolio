using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameData/SpawnData")]
public class SpawnData : ScriptableObject
{
    [Serializable]
    public struct SpawnInfo
    {
        public int actorId;
        public List<int> dropItemIds;
        public List<int> dropItemRate;
    }

    public List<SpawnInfo> spawnInfos;
    public List<int> spawnRate;
}
