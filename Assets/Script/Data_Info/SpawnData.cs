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

        [SerializeField, Tooltip("자동 계산된 합계")]
        private int dropItemRateSum;
        public void CalculateSum()
        {
            dropItemRateSum = 0;
            if (dropItemRate != null)
            {
                foreach (var r in dropItemRate)
                    dropItemRateSum += r;
            }

        }
    }

    public List<SpawnInfo> spawnInfos;
    public List<int> spawnRate;

    private void OnValidate()
    {
        // Inspector에서 값이 바뀔 때마다 자동 갱신
        for (int i = 0; i < spawnInfos.Count; i++)
        {
            var info = spawnInfos[i];
            info.CalculateSum();
            spawnInfos[i] = info; // struct이므로 다시 할당해야 값 반영됨
        }
    }
}
