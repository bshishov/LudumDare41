using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public GameObject Target;

    private NavMeshAgent _agent;

    public float BaseSpeed = 3.5f;
    public float OffMeshSpeed = 0.5f;
    
    void Start ()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (Target == null)
        {
            Debug.LogWarning("Target is not set");
        }
        else
        {
            _agent.SetDestination(Target.transform.position);
        }
	}
	
	void Update ()
    {
		if (_agent.isOnOffMeshLink)
        {
            _agent.speed = OffMeshSpeed;
        }
        else
        {
            _agent.speed = BaseSpeed;
        }
	}
}
