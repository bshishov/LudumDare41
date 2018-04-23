using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float RadiusMultiplier = 1.6f;
    public float VerticalOffset = 0.9f;
    public GameObject Cylinder;
    public GameObject Player;
    
    void OnDrawGizmos()
    {
        var lookPoint = Cylinder.transform.position;
        lookPoint.y = Player.transform.position.y;

        var playerFromCenter = Player.transform.position - lookPoint;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(playerFromCenter + Player.GetComponent<PlayerController>().MovingVector * 10, 0.3f);
    }

    void Start()
    {
        RecalculateLook();
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
        
        transform.position = Vector3.Lerp(transform.position, playerFromCenter + Player.GetComponent<PlayerController>().MovingVector * 8, 0.7f);
        transform.LookAt(lookPoint);
    }
}
