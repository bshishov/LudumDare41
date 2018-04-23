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

    public GameObject Cylinder;

    private CharacterController _controller;

    // центр, вокруг которого производится движение
    private Vector3 _movingCenter;
    private float _movingRadius;

    private float _verticalSpeed;
    private Vector3 _movingVector = new Vector3(0f, 0f, 0f);

    public Vector3 MovingVector
    {
        get { return _movingVector; }
    }
    
    void Start()
    {
        _controller = GetComponent<CharacterController>();

        Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer(Layers.Default));

        _movingCenter = Cylinder.transform.position;
        var playerFromCenter = transform.position - new Vector3(_movingCenter.x, transform.position.y, _movingCenter.z);
        _movingRadius = playerFromCenter.magnitude;
    }
    
    void Update()
    {
        _movingVector = Vector3.zero;
        CheckSideMoving();
        CheckJump();
        Move();
        Rotate();
    }

    void CheckSideMoving()
    {
        var side = Input.GetAxis("Horizontal");
        _movingVector = Camera.main.transform.right.normalized * side * Speed;
    }

    void Move()
    {
        if (_movingVector.magnitude <= 0.01f) return;

        var moveToFromCenter = transform.position + _movingVector - new Vector3(_movingCenter.x, transform.position.y, _movingCenter.z);
        moveToFromCenter = moveToFromCenter.normalized * _movingRadius;
        _movingVector = new Vector3(_movingCenter.x, transform.position.y, _movingCenter.z) + moveToFromCenter - transform.position;

        _controller.Move(_movingVector);
    }

    void CheckJump()
    {
        if (_controller.collisionFlags == CollisionFlags.Below)
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
        if (_movingVector.magnitude <= 0.01f) return;

        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.LookRotation(_movingVector.normalized),
            RotationSpeed);
    }
}
