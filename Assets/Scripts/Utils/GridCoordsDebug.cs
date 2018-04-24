#if DEBUG
using UnityEditor;
#endif

using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class GridCoordsDebug : MonoBehaviour
    {
        public bool Show = true;
        public bool ShowMappedPosition = false;
        public bool ShowTangent = false;
        public bool IsLeftTangent = true;

        private float _radius = 0.3f;

        void OnDrawGizmosSelected()
        {
            if (Show)
            {
                Gizmos.color = Color.red;
                var coords = PlacementGrid.Instance.WorldToCoords(transform.position);
                var p = PlacementGrid.Instance.CenterOfCell(coords);
                Gizmos.DrawSphere(p, _radius);
                Gizmos.DrawLine(transform.position, p);

#if DEBUG
                Handles.Label(p, coords.ToString(), "AssetLabel");
#endif
            }

            if (ShowMappedPosition)
            {
                Gizmos.color = Color.blue;
                var p = PlacementGrid.Instance.GridToWorld(PlacementGrid.Instance.WorldToGrid(transform.position));
                Gizmos.DrawSphere(p, _radius);
                Gizmos.DrawLine(transform.position, p);
            }

            if (ShowTangent)
            {
                var t = PlacementGrid.Instance.WorldTanget(transform.position, IsLeftTangent);
                Gizmos.DrawLine(transform.position, transform.position + t * 10);
            }
        }
    }
}
