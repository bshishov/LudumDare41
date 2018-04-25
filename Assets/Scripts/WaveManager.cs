using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts
{
    public class WaveManager : Singleton<WaveManager>
    {
        public List<Transform> SpawnPoints;
        public GameObject Target;
        public List<Wave> Waves;
        public float TimeBetweenWaves = 3f;

        public Wave CurrentWave { get; private set; }
        public bool WaveInProgress { get; private set; }
        public int WaveNumber { get { return _currentWaveIndex + 1; } }
        public int EnemiesOut { get; private set; }

        private int _currentWaveIndex = -1;

        void Start()
        {
            NextWave();
        }

        void NextWave()
        {
            if (WaveInProgress)
            {
                Debug.LogWarning("Can't start new wave, current one is in progress");
                return;
            }

            _currentWaveIndex++;
            if (_currentWaveIndex >= Waves.Count)
            {
                Debug.LogWarning("No more waves");
                return;
            }
                
            CurrentWave = Waves[_currentWaveIndex];
            EnemiesOut = 0;
            StartCoroutine(DoWave(CurrentWave));
        }

        private void SpawnEnemy(GameObject prefab)
        {
            foreach (var spawnPoint in SpawnPoints)
            {
                var enemy = Instantiate(prefab);
                enemy.GetComponent<NavMeshAgent>().Warp(spawnPoint.position);
                enemy.GetComponent<Enemy>().SetTarget(Target.transform.position);
                EnemiesOut++;
            }
        }

        private IEnumerator DoWave(Wave wave)
        {
            Debug.LogFormat("Wave {0}/{1} started!", WaveNumber, Waves.Count);
            WaveInProgress = true;
            foreach (var spawnEntry in wave.Items)
            {
                if(spawnEntry.EnemyPrefab == null || spawnEntry.Total == 0)
                    continue;

                for (var i = 0; i < spawnEntry.Total; i++)
                {
                    SpawnEnemy(spawnEntry.EnemyPrefab);
                    yield return new WaitForSeconds(spawnEntry.DelayBetween);
                }

                yield return new WaitForSeconds(spawnEntry.DelayBeforeNext);
            }
            WaveInProgress = false;
            Debug.LogFormat("Wave {0}/{1} ended!", WaveNumber, Waves.Count);
        }
    }
}
