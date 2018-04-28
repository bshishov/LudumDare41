using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float RadiusMultiplier = 1.6f;
    public float VerticalOffset = 0.9f;
    public GameObject Cylinder;
    public GameObject Player;

    private Vector3 _fromPosition;
    private Vector3 _targetPosition;
    private Vector3 _velocity;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_fromPosition, _targetPosition);
    }

    void Start()
    {
        RecalculateLook();
        _fromPosition = _targetPosition = transform.position;
    }
    
    void Update()
    {
        RecalculateLook();
    }

    void RecalculateLook()
    {
        var lookPoint = Cylinder.transform.position;
        lookPoint.y = Player.transform.position.y;

        var playerFromCenter = Player.transform.position - lookPoint;
        playerFromCenter *= RadiusMultiplier;
        playerFromCenter.y = lookPoint.y + VerticalOffset;

        _velocity = Vector3.Lerp(_velocity, 20f * (playerFromCenter - _fromPosition), 0.05f);

        _targetPosition = playerFromCenter + _velocity;
        _targetPosition.y = Mathf.Max(_targetPosition.y, 2f);

        transform.position = Vector3.Lerp(_fromPosition, _targetPosition, 0.8f);
        transform.LookAt(lookPoint);

        _fromPosition = playerFromCenter;
    }
}
