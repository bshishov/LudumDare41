using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.Scripts.Data;
using Assets.Scripts.Turrets;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerBuilder : MonoBehaviour
    {
        public Vector3 ReferencePoint;

        public float TurretRadiusOffset;
        public float TurretHeightOffset;
        public float Resource;
        public KeyCode BuildKey;
        public KeyCode OpenBuildingMenuKey;
        public KeyCode RotateKey;

        private Animator _animator;
        private CharacterController _characterController;
        private readonly Dictionary<Vector2Int, Turret> _placedTurrets = new Dictionary<Vector2Int, Turret>();

        void Start ()
        {
            _animator = GetComponent<Animator>();
            _characterController = GetComponent<CharacterController>();
        }
        
        void Update ()
        {
            if (Input.GetKeyDown(OpenBuildingMenuKey))
                UIBuildingMenu.Instance.Toggle();
            
            if (Input.GetKeyDown(BuildKey))
                TryBuild();

            if (Input.GetKeyDown(RotateKey))
            {
                var turret = TurretInRange();
                if (turret != null)
                    turret.ChangeDirection();
            }
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
            _animator.SetTrigger("Cast");
            PlaceTurret(activeSelection);
        }

        private bool CanPlace()
        {
            var c1 = PlacementGrid.Instance.WorldToCoords(transform.TransformPoint(ReferencePoint));

            // Is there is a turret already
            if (_placedTurrets.ContainsKey(c1))
                return false;

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

        private Turret TurretInRange()
        {
            var coords = PlacementGrid.Instance.WorldToCoords(transform.TransformPoint(ReferencePoint));
            if (_placedTurrets.ContainsKey(coords))
                return _placedTurrets[coords];

            return null;
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

            var coords = PlacementGrid.Instance.WorldToCoords(transform.TransformPoint(ReferencePoint));
            var cellCenter = PlacementGrid.Instance.CenterOfCell(coords, TurretRadiusOffset);
            var placementPosition = cellCenter + new Vector3(0, TurretHeightOffset, 0);

            var turretObj = GameObject.Instantiate(turret.Prefab, placementPosition, PlacementGrid.Instance.RotationAlongTangent(placementPosition));
            var turretCom = turretObj.GetComponent<Turret>();
            if (turretCom != null)
                _placedTurrets.Add(coords, turretCom);
        }

        void OnDrawGizmosSelected()
        {
            var c1 = PlacementGrid.Instance.WorldToCoords(transform.TransformPoint(ReferencePoint));
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
