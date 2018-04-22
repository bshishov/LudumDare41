using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnPointsManager : MonoBehaviour {

    public List<SpawnController> SpawnPoints;
    public GameObject Target;

    public float TimeBetweenWaves = 3f;

    private float _lastActionTime = float.MinValue;
    private int _currentWave = -1;

    private bool _weAreBetweenWaves = true;

    public bool WaveIsDone
    {
        get
        {
            return SpawnPoints.All(t => t.WaveIsDone);
        }
    }
    
	void Start () {
		
	}
	
	void Update () {
        if (!_weAreBetweenWaves && WaveIsDone)
        {
            _weAreBetweenWaves = true;
            _lastActionTime = Time.time;
            Debug.Log("Wave finished");
        }

		if (_weAreBetweenWaves && Time.time - _lastActionTime >= TimeBetweenWaves)
        {
            _weAreBetweenWaves = false;
            NextWave();
        }
	}

    void NextWave()
    {
        _currentWave++;
        foreach (var spawn in SpawnPoints)
        {
            spawn.StartWave(_currentWave, Target);
            Debug.Log("Wave started");
        }
    }
}
