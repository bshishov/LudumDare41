using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.AI;
using Ray = UnityEngine.Ray;

public class PlayerController : MonoBehaviour
{

    public float Speed = 0.07f;
    public float RotationSpeed = 0.3f;
    public float InitialJumpSpeed = 15.5f;

    public Vector3 MovingVector = new Vector3(0f, 0f, 0f);
    public GameObject Cylinder;

    private CharacterController _controller;
    private NavMeshAgent _navMeshAgent;

    // центр, вокруг которого производится движение
    private Vector3 _movingCenter;
    private float _movingRadius;

    private float _verticalSpeed;

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

        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer(Layers.Default));

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
        if (MovingVector.magnitude <= 0.01f) return;

        var moveToFromCenter = transform.position + MovingVector - new Vector3(_movingCenter.x, transform.position.y, _movingCenter.z);
        moveToFromCenter = moveToFromCenter.normalized * _movingRadius;
        MovingVector = new Vector3(_movingCenter.x, transform.position.y, _movingCenter.z) + moveToFromCenter - transform.position;

        _controller.Move(MovingVector);
    }

    void CheckJump()
    {
        if (_controller.isGrounded)
        {
            _verticalSpeed = 0f;
            var jump = Input.GetAxis("Jump");
            if (jump > 0)
            {
                _verticalSpeed = InitialJumpSpeed;
            }
        }

        if (_controller.collisionFlags == CollisionFlags.Above)
        {
            _verticalSpeed = 0f;
        }
        
        var verticalChange = Physics.gravity.y * Time.deltaTime / 10;
        _verticalSpeed += verticalChange;
        
        _controller.Move(new Vector3(0, _verticalSpeed, 0)); // падаем
    }

    void Rotate()
    {
        if (MovingVector.magnitude <= 0.01f) return;

        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.LookRotation(MovingVector.normalized),
            RotationSpeed);
    }
}
