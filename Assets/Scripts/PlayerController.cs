using Assets.Scripts.Sound;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        public float Speed = 0.07f;
        public float RotationSpeed = 0.3f;
        public float InitialJumpSpeed = 0.3f;
        public float JumpHoldSpeed = 0.01f;
        public GameObject Cylinder;
        public float IsGroundedDelay = 0.1f;

        public AudioClipWithVolume JumpSound;

        [HideInInspector]
        public bool IsLocked = false;

        private CharacterController _controller;
        private Animator _animator;

        // центр, вокруг которого производится движение
        private Vector3 _movingCenter;
        private float _movingRadius;
        private float _currentGroundedDelay;

        private Vector3 _movingVector = new Vector3(0f, 0f, 0f);

        public Vector3 MovingVector { get { return _movingVector; } }
        public float VerticalSpeed;

        void Start()
        {
            _controller = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();

            Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Layers.Player), LayerMask.NameToLayer(Layers.Default));

            _movingCenter = Cylinder.transform.position;
            var playerFromCenter = transform.position - new Vector3(_movingCenter.x, transform.position.y, _movingCenter.z);
            _movingRadius = playerFromCenter.magnitude;
        }
    
        void Update()
        {
            _currentGroundedDelay += Time.deltaTime;
            if (_controller.isGrounded)
                _currentGroundedDelay = 0;

            _movingVector = Vector3.zero;

            CheckSideMoving();
            CheckJump();
            Move();
            Rotate();
        }

        void CheckSideMoving()
        {
            var side = Input.GetAxis("Horizontal");
            if (IsLocked)
                side = 0f;
            _animator.SetFloat("Speed", Mathf.Abs(side));
            _movingVector = Camera.main.transform.right.normalized * side * Speed * Time.deltaTime;
        }

        void Move()
        {
            if (_movingVector.magnitude <= 0.01f) return;

            var moveToFromCenter = transform.position + _movingVector - new Vector3(_movingCenter.x, transform.position.y, _movingCenter.z);
            moveToFromCenter = moveToFromCenter.normalized * _movingRadius;
            _movingVector = new Vector3(_movingCenter.x, 0, _movingCenter.z) + moveToFromCenter - transform.position;

            _movingVector.y = 0f;
            _controller.Move(_movingVector);
        }

        void CheckJump()
        {
            var isGrounded = Physics.Raycast(transform.TransformPoint(_controller.center), Vector3.down, _controller.height * 0.6f,
                LayerMask.GetMask(Layers.Platform));
            var isCeilinged = Physics.Raycast(transform.TransformPoint(_controller.center), Vector3.up, _controller.height * 0.6f,
                LayerMask.GetMask(Layers.Platform));
            //var isGroundedCircle = Physics.SphereCast(transform.TransformPoint(_controller.center), _controller.radius * 1.1, Vector3.down);
            
            // сначала рассчитываем изменение вертикальной скорости
            var verticalChange = Physics.gravity.y * Time.deltaTime;
            VerticalSpeed += verticalChange;

            var jump = Input.GetButtonDown("Jump");
            if (IsLocked)
                jump = false;
            
            //if ((_controller.collisionFlags & CollisionFlags.Below) != 0)
            //if (!jump && _controller.isGrounded && _currentGroundedDelay < IsGroundedDelay)
            if (isGrounded)
            {
                VerticalSpeed = 0f;

                // если прыжок был - сразу подпрыгиваем
                if (jump)
                {
                    VerticalSpeed = InitialJumpSpeed;
                    if(JumpSound != null)
                        AudioManager.Instance.PlayClip(JumpSound);
                }
            }
//            else
//            {
//                // ?
//                if (jump)
//                {
//                    VerticalSpeed += JumpHoldSpeed;
//                }
//            }
            
            //if ((_controller.collisionFlags & (CollisionFlags.Above | CollisionFlags.Sides)) != 0)
            if (isCeilinged && VerticalSpeed > 0)
            {
                VerticalSpeed = 0f;
            }
        
            _controller.Move(new Vector3(0, VerticalSpeed * Time.deltaTime, 0)); // падаем
        }

        void Rotate()
        {
            var targetRotation = _movingVector.normalized;
            if (_movingVector.magnitude <= 0.01f)
                targetRotation = -Camera.main.transform.forward;

            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(targetRotation),
                RotationSpeed);
        }
    }
}
