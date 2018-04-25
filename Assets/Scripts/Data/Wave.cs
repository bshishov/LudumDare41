using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(fileName = "wave", menuName = "Gameplay/Wave")]
    public class Wave : ScriptableObject
    {
        public List<SpawnEntry> Items;

        public int TotalNumberOfEnemies()
        {
            return Items.Sum(spawnEntry => spawnEntry.Total);
        }
    }
}