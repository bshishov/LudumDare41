using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utils;
using UnityEngine;

#if DEBUG
using UnityEditor;
#endif

namespace Assets.Scripts
{
    public enum Direction
    {
        None,
        Left,
        Right
    }

    public enum AoEType
    {
        HorizontalPlatform,
        HorizontalDirectionalPlatform,
        HorizontalDirectional,
        Down,
        SphericalArea
    }

    public class Aoe : MonoBehaviour
    {
        [Range(0f, PlacementGrid.Segments)]
        public float Range = 3f;

        public AoEType AoEType;
        public Direction Direction;
        public Vector3 ReferenceOffset;

        [TagSelector]
        public string[] TargetTags;

        public List<GameObject> ObjectsInAoE { get { return _objectsInAoE; } }
        public List<Vector3> PathLeft { get { return _pathLeft; } }
        public List<Vector3> PathRight { get { return _pathRight; } }
        public List<Vector3> ActivePath
        {
            get
            {
                if (AoEType == AoEType.HorizontalDirectional || 
                    AoEType == AoEType.HorizontalDirectionalPlatform)
                {
                    if (Direction == Direction.Left)
                        return _pathLeft;
                    if (Direction == Direction.Right)
                        return _pathRight;
                }
                if (AoEType == AoEType.HorizontalPlatform)
                    return _pathMerged;
                return null;
            }
        }

        private readonly List<GameObject> _objectsInAoE = new List<GameObject>();

        // Only for side turrets
        private List<Vector3> _pathLeft;
        private List<Vector3> _pathRight;
        private List<Vector3> _pathMerged;
        private Mesh _aoeMeshLeft;
        private Mesh _aoeMeshRight;
        private Mesh _aoeMeshMerged;
        private MeshCollider _meshCollider;
        private SphereCollider _sphereCollider;
        private CapsuleCollider _capsuleCollider;
        
        void Start ()
        {
            RebuildAoe();
        }
        
        void Update ()
        {
            if(_objectsInAoE != null && _objectsInAoE.Count > 0)
                _objectsInAoE.RemoveAll(item => item == null);
        }
        
        [ContextMenu("Rebuild AoE")]
        public void RebuildAoe()
        {
            _objectsInAoE.Clear();

            if (_meshCollider != null)
                _meshCollider.enabled = false;

            if (_sphereCollider != null)
                _sphereCollider.enabled = false;

            if (_capsuleCollider != null)
                _capsuleCollider.enabled = false;

            if (AoEType == AoEType.HorizontalPlatform)
            {
                _pathLeft = BuildPath(Vector2.left);
                _pathRight = BuildPath(Vector2.right);
                _pathMerged = MergePath(_pathLeft, _pathRight);
                _aoeMeshMerged = ColliderUtils.MeshFromPath(WorldToLocalPath(_pathMerged), Vector3.left, 2f);
                SetAoeMesh(_aoeMeshMerged);
            }

            if (AoEType == AoEType.HorizontalDirectionalPlatform || 
                AoEType == AoEType.HorizontalDirectional)
            {
                if (this.Direction == Direction.Left)
                {
                    if (AoEType == AoEType.HorizontalDirectionalPlatform)
                        _pathLeft = BuildPath(Vector2.left);
                    else
                        _pathLeft = BuildPathNonPlatform(Vector2.left);
                    
                    if (_pathLeft != null)
                    {
                        _aoeMeshLeft = ColliderUtils.MeshFromPath(WorldToLocalPath(_pathLeft), Vector3.left, 2f);
                        SetAoeMesh(_aoeMeshLeft);
                    }
                }
                else if (this.Direction == Direction.Right)
                {
                    if (AoEType == AoEType.HorizontalDirectionalPlatform)
                        _pathRight = BuildPath(Vector2.right);
                    else
                        _pathRight = BuildPathNonPlatform(Vector2.right);

                    if (_pathRight != null)
                    {
                        _aoeMeshRight = ColliderUtils.MeshFromPath(WorldToLocalPath(_pathRight), Vector3.left, 2f);
                        SetAoeMesh(_aoeMeshRight);
                    }
                }
            }

            if (AoEType == AoEType.SphericalArea)
            {
                if (_sphereCollider == null)
                    _sphereCollider = gameObject.AddComponent<SphereCollider>();

                _sphereCollider.center = ReferenceOffset;
                _sphereCollider.enabled = true;
                _sphereCollider.radius = PlacementGrid.Instance.GridRangeToWorld(Range);
                _sphereCollider.isTrigger = true;
            }

            if (AoEType == AoEType.Down)
            {
                if (_capsuleCollider == null)
                    _capsuleCollider = gameObject.AddComponent<CapsuleCollider>();

                _capsuleCollider.radius = PlacementGrid.Instance.GridRangeToWorld(Range);
                _capsuleCollider.height = transform.position.y + Range;
                _capsuleCollider.center = new Vector3(0, -transform.position.y / 2 - Range / 2, 0) + ReferenceOffset;
                _capsuleCollider.enabled = true;
                _capsuleCollider.isTrigger = true;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (TargetTags.Contains(other.tag))
            {
                if (!_objectsInAoE.Contains(other.gameObject))
                    _objectsInAoE.Add(other.gameObject);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (_objectsInAoE.Contains(other.gameObject))
                _objectsInAoE.Remove(other.gameObject);
        }

        public List<Vector3> BuildPath(Vector2 direction)
        {
            var padding = new Vector3(0, PlacementGrid.Instance.SegmentHeight / 2, 0);
            var points = new List<Vector3>();
            var gridPoint = PlacementGrid.Instance.WorldToGrid(transform.TransformPoint(ReferenceOffset));
            points.Add(PlacementGrid.Instance.GridToWorld(gridPoint));

            for (var i = 0; i < Range; i++)
            {
                gridPoint += direction;
                var sampleWorld = PlacementGrid.Instance.GridToWorld(gridPoint);

                RaycastHit hit;
                var ray = new UnityEngine.Ray(sampleWorld + padding * 2, Vector3.down);
                if (Physics.Raycast(ray, out hit, PlacementGrid.Instance.SegmentHeight * 2.5f, LayerMask.GetMask(Layers.Platform)))
                {
                    var pathPoint = hit.point + padding;
                    points.Add(pathPoint);
                    gridPoint = PlacementGrid.Instance.WorldToGrid(pathPoint);
                }
                else
                {
                    break;
                }
            }

            if (points.Count == 1)
                return null;

            return points;
        }

        public List<Vector3> BuildPathNonPlatform(Vector2 direction)
        {
            var gridPoint = PlacementGrid.Instance.WorldToGrid(transform.TransformPoint(ReferenceOffset));
            var points = new List<Vector3> { PlacementGrid.Instance.GridToWorld(gridPoint) };

            for (var i = 0; i < Range; i++)
            {
                gridPoint += direction;
                points.Add(PlacementGrid.Instance.GridToWorld(gridPoint));
            }

            return points;
        }

        public List<Vector3> WorldToLocalPath(List<Vector3> path)
        {
            return path.Select(v => transform.InverseTransformPoint(v)).ToList();
        }

        public List<Vector3> MergePath(List<Vector3> pathLeft, List<Vector3> pathRight)
        {
            if (pathRight == null)
                return pathLeft;

            if (pathLeft == null)
                return pathRight;

            var p = new List<Vector3>();
           
            for (var i = pathRight.Count - 1; i >= 0; i--)
                p.Add(pathRight[i]);

            for (var i = 1; i < pathLeft.Count; i++)
                p.Add(pathLeft[i]);

            return p;
        }

        private void SetAoeMesh(Mesh mesh)
        {
            if (mesh == null)
            {
                Debug.LogWarning("Empty AOE mesh", gameObject);
                return;
            }

            if (_meshCollider == null)
                _meshCollider = gameObject.AddComponent<MeshCollider>();

            _meshCollider.enabled = true;
            _meshCollider.sharedMesh = mesh;
            _meshCollider.convex = true;
            _meshCollider.isTrigger = true;
        }

        void DrawGizmosPath(List<Vector3> path, Color color)
        {
            if(path == null || path.Count < 2)
                return;

            Gizmos.color = color;
            for (var i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
            }
        }

        void OnDrawGizmosSelected()
        {
            if (this.AoEType == AoEType.HorizontalDirectionalPlatform || 
                this.AoEType == AoEType.HorizontalDirectional)
                DrawGizmosPath(ActivePath, Color.cyan);

            if (this.AoEType == AoEType.HorizontalPlatform)
                DrawGizmosPath(_pathMerged, Color.cyan);
#if DEBUG
            Handles.Label(transform.position, string.Format("In range: {0}", _objectsInAoE.Count), "textField");
#endif
        }
    }
}
