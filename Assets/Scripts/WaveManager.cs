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
        public float TimeBetweenWaves = 20f;

        public Wave CurrentWave { get; private set; }
        public bool WaveInProgress { get; private set; }
        public int WaveNumber { get { return _currentWaveIndex + 1; } }
        public int EnemiesOut { get; private set; }
        public float TimeToNextWave { get; private set; }

        public float Percentage
        {
            get
            {
                if(CurrentWave != null)
                    return Mathf.Clamp01((float) EnemiesOut / CurrentWave.TotalNumberOfEnemies());
                return 0f;
            }
        }

        private int _currentWaveIndex = -1;

        void Start()
        {
            NextWave();
        }

        void Update()
        {
            if (!WaveInProgress && _currentWaveIndex < Waves.Count)
            {
                TimeToNextWave -= Time.deltaTime;
                if (TimeToNextWave < 0)
                {
                    TimeToNextWave = 0;
                    NextWave();
                }
            }
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

        private void SpawnEnemy(Transform spawnPoint, GameObject prefab)
        {
            var enemy = Instantiate(prefab);
            enemy.GetComponent<NavMeshAgent>().Warp(spawnPoint.position);
            enemy.GetComponent<Enemy>().SetTarget(Target.transform.position);
            EnemiesOut++;
        }

        private void SpawnEnemy(int spawnerIndex, GameObject prefab)
        {
            if (spawnerIndex < 0 || spawnerIndex > SpawnPoints.Count - 1)
            {
                Debug.LogFormat("SpawnerIndex is {0}. Spawning on both waves", spawnerIndex);
                foreach (var spawnPoint in SpawnPoints)
                    SpawnEnemy(spawnPoint, prefab);
            }
            else
            {
                SpawnEnemy(SpawnPoints[spawnerIndex], prefab);
            }
        }

        private IEnumerator DoWave(Wave wave)
        {
            Debug.LogFormat("Wave {0}/{1} started!", WaveNumber, Waves.Count);
            WaveInProgress = true;
            foreach (var spawnEntry in wave.Items)
            {
                if(spawnEntry.EnemyPrefab == null || spawnEntry.NumberOfThisType == 0)
                    continue;

                for (var i = 0; i < spawnEntry.NumberOfThisType; i++)
                {
                    SpawnEnemy(spawnEntry.SpawnIndex, spawnEntry.EnemyPrefab);
                    yield return new WaitForSeconds(spawnEntry.DelayBetween);
                }

                yield return new WaitForSeconds(spawnEntry.DelayBeforeNext);
            }
            WaveInProgress = false;
            Debug.LogFormat("Wave {0}/{1} ended!", WaveNumber, Waves.Count);
            TimeToNextWave = this.TimeBetweenWaves;
        }
    }
}
