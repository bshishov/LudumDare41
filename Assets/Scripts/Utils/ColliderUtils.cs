using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class ColliderUtils
    {
        public static Mesh MeshFromPath(List<Vector3> path, Vector3 direction, float thickness = 1f)
        {
            var mesh = new Mesh();

            var vertices = new Vector3[path.Count * 2];
            var uvs = new Vector2[path.Count * 2];
            var normals = new Vector3[path.Count * 2];

            var vIdx = 0;
            for (var i = 0; i < path.Count; i++)
            {
                var p = path[i];

                // UPPER
                vertices[vIdx] = p + direction * thickness * 0.5f;
                uvs[vIdx] = new Vector2((float)i / (path.Count - 1), 1f);
                normals[vIdx] = Vector3.back;
                vIdx += 1;

                //LOWER
                vertices[vIdx] = p - direction * thickness * 0.5f;
                uvs[vIdx] = new Vector2((float)i / (path.Count - 1), 0f);
                normals[vIdx] = Vector3.up;
                vIdx += 1;
            }

            var tris = new int[(path.Count - 1) * 6];

            for (var i = 0; i < path.Count - 1; i++)
            {
                tris[i * 6 + 0] = i * 2 + 0;
                tris[i * 6 + 1] = i * 2 + 1;
                tris[i * 6 + 2] = i * 2 + 2;
                tris[i * 6 + 3] = i * 2 + 3;
                tris[i * 6 + 4] = i * 2 + 2;
                tris[i * 6 + 5] = i * 2 + 1;
            }

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = tris;
            mesh.normals = normals;
            mesh.RecalculateBounds();
            
            return mesh;
        }
        
        public static Vector3 ClosestPointOnPath(Vector3 from, List<Vector3> path)
        {
            var count = path.Count;
            if (count == 0)
                return from;

            if (count == 1)
                return path[0];

            var nearestDist = float.MaxValue;
            var nearestPoint = from;

            for (var i = 0; i < count - 1; i++)
            {
                var p = ClosestPointOnSegment(from, path[i], path[i + 1]);
                var dist = Vector3.Distance(from, p);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearestPoint = p;
                }
            }

            return nearestPoint;
        }

        public static Vector3 ClosestPointOnSegment(Vector3 p, Vector3 a, Vector3 b)
        {
            var dir = b - a;
            return Vector3.Lerp(a, b, Vector3.Dot(p - a, dir) / dir.sqrMagnitude);
        }
    }
}
