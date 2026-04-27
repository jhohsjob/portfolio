using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameDefinition/SpawnDefinition")]
public class SpawnDefinition : ScriptableObject
{
    [Serializable]
    public class DropItemEntry
    {
        public int roleId;
        public int spawnRate;
    }

    [Serializable]
    public class EnemyEntry
    {
        [SerializeField, ReadOnly, Tooltip("자동 계산된 합계")]
        private int _dropItemRateSum;

        public int roleId;
        public int spawnRate;
        public List<DropItemEntry> dropItemEntries = new();

        public void CalculateSum()
        {
            _dropItemRateSum = 0;

            if (dropItemEntries == null)
            {
                return;
            }

            foreach (var dropItem in dropItemEntries)
            {
                _dropItemRateSum += dropItem.spawnRate;
            }
        }
    }
    
    [SerializeField, ReadOnly, Tooltip("자동 계산된 합계")]
    private int _enemyRateSum;

    public List<EnemyEntry> enemyEntries = new();

    private void OnValidate()
    {
        _enemyRateSum = 0;

        if (enemyEntries == null)
        {
            return;
        }

        foreach (var enemyEntry in enemyEntries)
        {
            enemyEntry.CalculateSum();

            _enemyRateSum += enemyEntry.spawnRate;
        }
    }
}