using UnityEngine;

namespace Assets.Scripts
{
    public class Soul : MonoBehaviour
    {
        public float FlyTime = 2f;
        public float MaxSpeed = 10f;
        public float PickProximity = 1f;
        public Vector3 TargetOffset = new Vector3(0, 2, 0);
        public int Value = 0;
        private PlayerBuilder _player;
        private Vector3 _velocity;

        void Start ()
        {
            _player = GameObject.FindObjectOfType<PlayerBuilder>();
        }
        
        void Update ()
        {
            if (_player == null)
                return;

            var playerPosition = _player.transform.position + TargetOffset;
            transform.position = Vector3.SmoothDamp(transform.position, playerPosition, ref _velocity, FlyTime, 10f);

            if (Vector3.Distance(playerPosition, transform.position) < PickProximity)
            {
                 _player.AddSouls(Value);
                Destroy(gameObject);
            }
        }
    }
}
