using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlacementGrid : Singleton<PlacementGrid>
    {
        public const int Segments = 72;
        public const float TwoPI = Mathf.PI * 2f;
        public const float SegmentAngle = TwoPI / Segments;
        

        public float Radius = 20;
        public float PlatformDepth = 2f;
        public float SegmentHeight = 2f;
        public int Height = 100;

        private readonly Color _color = new Color(0f, 0f, 0f, 0.3f);

        public void OnDrawGizmos()
        {
            Gizmos.color = _color;
            var startingPoint = transform.position;
            for (var i = 0; i < Height; i++)
            {
                var h = i * SegmentHeight;
                for (var j = 0; j < Segments; j++)
                {
                    var f1 = j * SegmentAngle;
                    var f2 = (j + 1) * SegmentAngle;
                    var p1 = new Vector3(Mathf.Sin(f1) * Radius, h, Mathf.Cos(f1) * Radius);
                    var p2 = new Vector3(Mathf.Sin(f2) * Radius, h, Mathf.Cos(f2) * Radius);

                    var p3 = p1 + new Vector3(0, SegmentHeight, 0);
                    Gizmos.DrawLine(startingPoint + p1, startingPoint + p2);
                    Gizmos.DrawLine(startingPoint + p1, startingPoint + p3);
                }
            }
        }

        public Vector3 CenterOfCell(Vector2Int p)
        {
            return GridToWorld(new Vector2(p.x + 0.5f, p.y + 0.5f));
        }

        public Vector3 CenterOfCell(Vector2Int p, float offset)
        {
            return GridToWorld(new Vector2(p.x + 0.5f, p.y + 0.5f), offset);
        }

        public Vector2Int WorldToCoords(Vector3 p)
        {
            var angle = Mathf.Atan2(p.x, p.z) % TwoPI / TwoPI;
            return new Vector2Int(Mathf.FloorToInt(angle * Segments), Mathf.FloorToInt(p.y / SegmentHeight));
        }

        public Vector2 WorldToGrid(Vector3 p)
        {
            var angle = Mathf.Atan2(p.x, p.z) % TwoPI / TwoPI;
            return new Vector2(angle * Segments, p.y / SegmentHeight);
        }

        public Vector3 GridToWorld(Vector2 p)
        {
            return new Vector3(Mathf.Sin(p.x * SegmentAngle) * Radius, p.y * SegmentHeight, Mathf.Cos(p.x * SegmentAngle) * Radius);
        }

        public Vector3 GridToWorld(Vector2 p, float offset)
        {
            return new Vector3(Mathf.Sin(p.x * SegmentAngle) * (Radius + offset), p.y * SegmentHeight, Mathf.Cos(p.x * SegmentAngle) * (Radius + offset));
        }
    }
}
