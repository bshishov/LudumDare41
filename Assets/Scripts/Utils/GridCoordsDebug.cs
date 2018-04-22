using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class GridCoordsDebug : MonoBehaviour
    {
        public bool Show = true;
        public bool ShowMappedPosition = false;

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

                Handles.Label(p, coords.ToString(), "AssetLabel");
            }

            if (ShowMappedPosition)
            {
                Gizmos.color = Color.blue;
                var p = PlacementGrid.Instance.GridToWorld(PlacementGrid.Instance.WorldToGrid(transform.position));
                Gizmos.DrawSphere(p, _radius);
                Gizmos.DrawLine(transform.position, p);
            }
        }
    }
}
