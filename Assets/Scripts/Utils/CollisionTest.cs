using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    [RequireComponent(typeof(MeshCollider))]
    public class CollisionTest : MonoBehaviour
    {
        public Transform Target;
        public List<Vector3> Path;
        private MeshCollider _collider;

        void Start()
        {
            _collider = GetComponent<MeshCollider>();
            if (Path != null && Path.Count > 1)
            {
                _collider.sharedMesh = ColliderUtils.MeshFromPath(Path, Vector3.left);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            Debug.Log("Trigger Enter");
        }

        void OnTriggerExit(Collider other)
        {
            Debug.Log("Trigger Exit");
        }

        void OnCollisionEnter(Collision other)
        {
            Debug.Log("Collision Enter");
        }

        void OnCollisionExit(Collision other)
        {
            Debug.Log("Collision Exit");
        }

        void OnDrawGizmos()
        {
            if (Path != null && Path.Count > 1)
            {
                Gizmos.color = Color.cyan;
                for (var i = 0; i < Path.Count - 1; i++)
                {
                    Gizmos.DrawLine(transform.TransformPoint(Path[i]), transform.TransformPoint(Path[i + 1]));
                }

                if (Target != null)
                {
                    var p = transform.TransformPoint(ColliderUtils.ClosestPointOnPath(transform.InverseTransformPoint(Target.position), Path));
                    Gizmos.DrawSphere(p, 0.2f);
                    Gizmos.DrawLine(p, Target.position);
                }
            }
        }
    }
}
