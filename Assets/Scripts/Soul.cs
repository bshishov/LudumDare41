using UnityEngine;

namespace Assets.Scripts
{
    public class Soul : MonoBehaviour
    {
        public float FlyTime = 2f;
        public float MaxSpeed = 10f;
        private GameObject _player;
        private Vector3 _velocity;

        void Start ()
        {
            _player = GameObject.FindGameObjectWithTag(Tags.Player);
        }
        
        void Update ()
        {
            if (_player == null)
                return;

            transform.position = Vector3.SmoothDamp(transform.position, _player.transform.position, ref _velocity,
                FlyTime, 10f);
        }
    }
}
