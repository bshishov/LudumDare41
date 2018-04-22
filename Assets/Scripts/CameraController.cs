using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float RadiusMultiplier = 1.6f;
    public float VerticalOffset = 0.9f;
    public GameObject Cylinder;
    public GameObject Player;
    
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

        transform.position = playerFromCenter;
        transform.LookAt(lookPoint);
    }
}
