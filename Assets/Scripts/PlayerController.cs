using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    public float Speed = 0.07f;
    public float RotationSpeed = 0.5f;
    public float JumpForce = 15.5f;

    public Vector3 MovingVector = new Vector3(0f, 0f, 0f);
    public GameObject Cylinder;

    private CharacterController _controller;
    private NavMeshAgent _navMeshAgent;

    // центр, вокруг которого производится движение
    private Vector3 _movingCenter;
    private float _movingRadius;

    private float _currentJumpForce;

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(new Ray(transform.position, MovingVector));
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + MovingVector * Speed, 0.05f);
        Gizmos.DrawRay(new Ray(Camera.main.transform.position, Camera.main.transform.right));
    }
    
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _movingCenter = Cylinder.transform.position;
        var playerFromCenter = transform.position - new Vector3(_movingCenter.x, 0, _movingCenter.z);
        _movingRadius = playerFromCenter.magnitude;
    }
    
    void Update()
    {
        MovingVector = Vector3.zero;
        CheckSideMoving();
        CheckJump();
        Move();
        Rotate();
    }

    void CheckSideMoving()
    {
        var side = Input.GetAxis("Horizontal");
        MovingVector = Camera.main.transform.right.normalized * side * Speed;
    }

    void Move()
    {
        var verticalChange = Physics.gravity * Time.deltaTime;
        var beforeFalling = transform.position.y;
        _controller.Move(new Vector3(0, _currentJumpForce * Time.deltaTime, 0) + verticalChange); // падаем
        if (_controller.isGrounded)
            _currentJumpForce = 0f;
        else
            _currentJumpForce = Mathf.Max(0f, _currentJumpForce - verticalChange.magnitude);

        if (MovingVector.magnitude <= 0.01f) return;

        var moveToFromCenter = transform.position + MovingVector - new Vector3(_movingCenter.x, transform.position.y, _movingCenter.z);
        moveToFromCenter = moveToFromCenter.normalized * _movingRadius;
        MovingVector = new Vector3(_movingCenter.x, transform.position.y, _movingCenter.z) + moveToFromCenter - transform.position;

        _controller.Move(MovingVector);
    }

    void CheckJump()
    {
        if (!_controller.isGrounded) return;
        var jump = Input.GetAxis("Jump");
        if (jump > 0)
        {
            _currentJumpForce = JumpForce;
        }
    }

    void Rotate()
    {
        if (MovingVector.magnitude <= 0.01f) return;

        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.LookRotation(MovingVector.normalized),
            RotationSpeed);
    }
}
