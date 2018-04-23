using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIFollowSceneObject : MonoBehaviour
    {
        public GameObject Target;
        public Vector3 Offset;

        private Camera _main;

        void Start()
        {
            _main = Camera.main;
        }

        void Update()
        {
            if (Target != null)
            {
                var scr = _main.WorldToScreenPoint(Target.transform.position + Offset);
                transform.position = new Vector3(scr.x, scr.y, transform.position.z);
            }
        }

        public void SetTarget(GameObject target)
        {
            Target = target;
            if (Target != null)
            {
                var scr = _main.WorldToScreenPoint(Target.transform.position + Offset);
                transform.position = new Vector3(scr.x, scr.y, transform.position.z);
            }
        }
    }
}
