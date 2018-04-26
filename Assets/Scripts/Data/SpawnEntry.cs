using System;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class SpawnEntry
    {
        public GameObject EnemyPrefab;
        public int NumberOfThisType = 1;
        public float DelayBetween = 0.1f;
        public float DelayBeforeNext = 0.5f;
        public int SpawnIndex = 0;
    }
}