using System;
using System.Runtime.InteropServices;
using Assets.Scripts.Data;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerBuilder : MonoBehaviour
    {
        public float TurretRadiusOffset;
        public float TurretHeightOffset;
        public float Resource;
        public KeyCode BuildKey;
        public KeyCode OpenBuildingMenuKey;

        private CharacterController _characterController;

        void Start ()
        {
            _characterController = GetComponent<CharacterController>();
        }
        
        void Update ()
        {
            if (Input.GetKeyDown(OpenBuildingMenuKey))
                UIBuildingMenu.Instance.Toggle();
            
            if (Input.GetKeyDown(BuildKey))
                TryBuild();
        }

        private void TryBuild()
        {
            if(!UIBuildingMenu.Instance.IsActive)
                return;

            var activeSelection = UIBuildingMenu.Instance.ActiveSelection;
            if (activeSelection == null)
                return;
            if (Resource < activeSelection.Cost)
            {
                UINotifications.Instance.Show(transform, "Not enough souls", Color.red);
                return;
            }

            if (!CanPlace())
            {
                UINotifications.Instance.Show(transform, "Can't build here", Color.red);
                return;
            }

            Resource -= activeSelection.Cost;
            PlaceTurret(activeSelection);
        }

        private bool CanPlace()
        {
            var c1 = PlacementGrid.Instance.WorldToCoords(transform.position);
            var cLeft = c1 + Vector2Int.left;
            var cRight = c1 + Vector2Int.right;

            var pCenter = PlacementGrid.Instance.CenterOfCell(c1);
            var pLeft = PlacementGrid.Instance.CenterOfCell(cLeft);
            var pRight = PlacementGrid.Instance.CenterOfCell(cRight);

            if (!HitPlatform(pCenter))
                return false;

            if (!HitPlatform(pLeft))
                return false;

            if (!HitPlatform(pRight))
                return false;

            return true;
        }

        private bool HitPlatform(Vector3 p)
        {
            return Physics.Raycast(p, Vector3.down, PlacementGrid.Instance.SegmentHeight, LayerMask.GetMask(Layers.Platform));
        }

        private void PlaceTurret(TurretInfo turret)
        {
            if(!_characterController.isGrounded)
                return;

            if (turret.Prefab == null)
            {
                Debug.LogWarning("No prefab for turret");
                return;
            }

            var coords = PlacementGrid.Instance.CenterOfCell(PlacementGrid.Instance.WorldToCoords(transform.position), TurretRadiusOffset);
            var placementPosition = coords + new Vector3(0, TurretHeightOffset, 0);

            GameObject.Instantiate(turret.Prefab, placementPosition, Quaternion.identity);
        }

        void OnDrawGizmosSelected()
        {
            var c1 = PlacementGrid.Instance.WorldToCoords(transform.position);
            var cLeft = c1 + Vector2Int.left;
            var cRight = c1 + Vector2Int.right;

            var pCenter = PlacementGrid.Instance.CenterOfCell(c1);
            var pLeft = PlacementGrid.Instance.CenterOfCell(cLeft);
            var pRight = PlacementGrid.Instance.CenterOfCell(cRight);

            Gizmos.DrawSphere(pCenter, 0.5f);
            Gizmos.DrawSphere(pLeft, 0.5f);
            Gizmos.DrawSphere(pRight, 0.5f);

            Gizmos.DrawLine(pCenter, pCenter + Vector3.down * PlacementGrid.Instance.SegmentHeight);
            Gizmos.DrawLine(pLeft, pLeft + Vector3.down * PlacementGrid.Instance.SegmentHeight);
            Gizmos.DrawLine(pRight, pRight + Vector3.down * PlacementGrid.Instance.SegmentHeight);
        }
    }
}
