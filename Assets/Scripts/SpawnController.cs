using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnEntry
{
    public GameObject EnemyPrefab;
    public float DelayBeforeNext;
}

[Serializable]
public class Wave
{
    public List<SpawnEntry> Items;
}

public class SpawnController : MonoBehaviour {
    
    public List<Wave> Waves = new List<Wave>();


    private float _lastActionTime = float.MinValue;
    private Wave _currentWave = null;
    private int _currentInWaveIndex = -1;
    private GameObject _target;
    
    public bool WaveIsDone
    {
        get
        {
            return _currentWave == null;
        }
    }

    void Start () {
		
	}
	
	void Update () {

        if (_currentWave == null) return;

        if (_currentInWaveIndex >= _currentWave.Items.Count - 1)
        {
            _lastActionTime = float.MinValue;
            _currentWave = null;
            _currentInWaveIndex = -1;
            return;
        }

        if (_currentInWaveIndex < 0 || Time.time - _lastActionTime >= _currentWave.Items[_currentInWaveIndex].DelayBeforeNext)
        {
            _currentInWaveIndex++;

            Debug.Log(String.Format("Spawn {0}", _currentWave.Items[_currentInWaveIndex].EnemyPrefab.name));
            var enemy = Instantiate(_currentWave.Items[_currentInWaveIndex].EnemyPrefab);
            enemy.transform.position = transform.position;
            enemy.GetComponent<EnemyController>().Target = _target;

            _lastActionTime = Time.time;
        }
        else
        {
            return;
        }
	}

    public bool StartWave(int index, GameObject target)
    {
        if (index >= Waves.Count) return false;
        _currentWave = Waves[index];
        _target = target;
        return true;
    }
}
