using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class PathFollow : MonoBehaviour
    {
        public float Speed = 1f;
        public Vector3[] Path;
        public bool DestroyAtTheEnd = true;

        private int _aIndex = 0;
        private float _t = 0f;
        
        void Update ()
        {
            if(Path == null || Path.Length < 2)
                return;

            if (_aIndex == Path.Length - 1)
            {
                if(DestroyAtTheEnd)
                    Destroy(gameObject);
                return;
            }

            var a = Path[_aIndex];
            var b = Path[_aIndex + 1];
            var d = Vector3.Distance(a, b);

            _t += Speed * Time.deltaTime / d;

            transform.position = Vector3.Lerp(a, b, _t);

            if (_t > 1f)
            {
                _t = 0f;
                _aIndex += 1;
            }
        }

        public void Go(Vector3[] path)
        {
            if (path == null || path.Length < 2)
            {
                Debug.LogWarning("Incorrect path for path follow");
                return;
            }

            Path = path;
            _t = 0;
            _aIndex = 0;
        }
    }
}
