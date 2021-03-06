﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assets.Scripts.Data;
using Assets.Scripts.Sound;
using Assets.Scripts.Turrets;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerBuilder : MonoBehaviour
    {
        public Vector3 ReferencePoint;

        public float TurretRadiusOffset;
        public float TurretHeightOffset;
        public float Resource;
        [Range(0, 1)]
        public float Cashback;

        [Header("Controls")]
        public string BuildActivateButton;
        public string OpenBuildingMenuButton;
        public string RotateButton;
        public string DestroyButton;

        [Header("Misc")]
        public Text ResourceText;

        [Header("Audio")]
        public AudioClipWithVolume OpenBuildingMenu;
        public AudioClipWithVolume SoulPickupSound;
        public AudioClipWithVolume PlaceTurretSound;
        public AudioClipWithVolume DestroyTurretSound;
        public AudioClipWithVolume RotateTurretSound;
        public AudioClipWithVolume ErrorSound;

        private Animator _animator;
        private CharacterController _characterController;
        private PlayerController _playerController;
        private readonly Dictionary<Vector2Int, Turret> _placedTurrets = new Dictionary<Vector2Int, Turret>();
        private Turret _currentTurret;

        void Start ()
        {
            _animator = GetComponent<Animator>();
            _characterController = GetComponent<CharacterController>();
            _playerController = GetComponent<PlayerController>();
            UpdateResourceUI();
        }
        
        void Update ()
        {
            var turret = TurretInRange();
            if (turret != null)
            {
                if (turret != _currentTurret)
                {
                    _currentTurret = turret;
                    OnTurretInRange();
                }
                
                UITurretInfo.Instance.CoolDownPercentage = _currentTurret.CooldownPercentage;
                UITurretInfo.Instance.ArrowDirection = _currentTurret.Direction;
                UITurretInfo.Instance.ShowActivateText = _currentTurret.FireMode == Turret.ActivationType.Manual &&
                                                         _currentTurret.CooldownPercentage < 0.01f;
            }
            else
            {
                if (_currentTurret != null)
                {
                    OnTurretOutOfRange();
                    _currentTurret = null;
                }
            }

            if (UIBuildingMenu.Instance.IsActive)
            {
                if (Input.GetButtonDown(OpenBuildingMenuButton) || Input.GetButtonDown("Cancel"))
                {
                    _playerController.IsLocked = false;
                    UIBuildingMenu.Instance.Hide();
                }

                if (UIBuildingMenu.Instance.ActiveSelection != null)
                {
                    if (Input.GetButtonDown(BuildActivateButton) || Input.GetButtonDown("Submit"))
                    {
                        TryBuild();
                    }
                }
            }
            else
            {
                if (Input.GetButtonDown(OpenBuildingMenuButton))
                {
                    if (CanPlace())
                    {
                        _playerController.IsLocked = true;
                        if(OpenBuildingMenu != null)
                            AudioManager.Instance.PlayClip(OpenBuildingMenu);
                        UIBuildingMenu.Instance.Show();
                    }
                    else
                    {
                        UINotifications.Instance.Show(transform, "Can't build here", Color.red, yOffset: 2f);

                        if (ErrorSound != null)
                            AudioManager.Instance.PlayClip(ErrorSound);
                    }
                }

                if (Input.GetButtonDown(BuildActivateButton))
                {
                    if (turret != null)
                    {
                        turret.Activate();
                        _animator.SetTrigger("Cast");
                    }
                }
            }


            if (Input.GetButtonDown(RotateButton))
            {
                if (turret != null)
                {
                    turret.ChangeDirection();

                    if(RotateTurretSound != null)
                        AudioManager.Instance.PlayClip(RotateTurretSound);
                }
            }

            if (Input.GetButtonDown(DestroyButton))
            {
                TryDestroy();
            }
        }

        public void AddSouls(int amount)
        {
            if (amount > 0)
            {
                UINotifications.Instance.Show(transform, amount.ToString(), Color.cyan, yOffset: 2f);
                Resource += amount;
                UpdateResourceUI();

                if(SoulPickupSound != null)
                    AudioManager.Instance.PlayClip(SoulPickupSound);
            }
        }

        private void UpdateResourceUI()
        {
            if (ResourceText != null)
                ResourceText.text = Resource.ToString();
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
                UINotifications.Instance.Show(transform, "Not enough souls", Color.red, yOffset: 2f);

                if (ErrorSound != null)
                    AudioManager.Instance.PlayClip(ErrorSound);

                return;
            }

            if (!CanPlace())
            {
                UINotifications.Instance.Show(transform, "Turret can't be placed here", Color.red, yOffset: 2f);

                if (ErrorSound != null)
                    AudioManager.Instance.PlayClip(ErrorSound);
                return;
            }

            if (PlaceTurret(activeSelection))
            {
                UIBuildingMenu.Instance.Hide();
                _playerController.IsLocked = false;

                Resource -= activeSelection.Cost;
                UpdateResourceUI();
                _animator.SetTrigger("Cast");
                
                if(PlaceTurretSound != null)
                    AudioManager.Instance.PlayClip(PlaceTurretSound);
            }
        }

        private bool CanPlace()
        {
            //if (!_characterController.isGrounded)
            //  return false;

            var c1 = PlacementGrid.Instance.WorldToCoords(transform.TransformPoint(ReferencePoint));

            // Is there is a turret already
            if (_placedTurrets.ContainsKey(c1))
                return false;

            var cLeft = c1 + Vector2Int.left;
            var cRight = c1 + Vector2Int.right;

            var pCenter = PlacementGrid.Instance.CenterOfCell(c1);
            var pLeft = Vector3.Lerp(pCenter, PlacementGrid.Instance.CenterOfCell(cLeft), 0.45f);
            var pRight = Vector3.Lerp(pCenter, PlacementGrid.Instance.CenterOfCell(cRight), 0.45f);
            
            if (!HitPlatform(pCenter))
                return false;

            var leftHit = HitPlatform(pLeft);
            var rightHit = HitPlatform(pRight);

            return leftHit && rightHit;
            //return leftHit || rightHit;
        }

        private bool HitPlatform(Vector3 p)
        {
            RaycastHit hit;
            var res = Physics.Raycast(new UnityEngine.Ray(p, Vector3.down), out hit, PlacementGrid.Instance.SegmentHeight * 0.52f, LayerMask.GetMask(Layers.Platform));
            if (!res)
                return false;

            return Vector3.Angle(hit.normal, Vector3.up) < 10;
        }

        private Turret TurretInRange()
        {
            var coords = PlacementGrid.Instance.WorldToCoords(transform.TransformPoint(ReferencePoint));
            if (_placedTurrets.ContainsKey(coords))
                return _placedTurrets[coords];

            return null;
        }

        private void TryDestroy()
        {
            var coords = PlacementGrid.Instance.WorldToCoords(transform.TransformPoint(ReferencePoint));
            if (_placedTurrets.ContainsKey(coords))
            {
                var t = _placedTurrets[coords];
                if (t.Info != null)
                    AddSouls(Mathf.CeilToInt(t.Info.Cost * Cashback));
                
                if (_currentTurret == t)
                {
                    _currentTurret = null;
                    OnTurretOutOfRange();
                }

                Destroy(t.gameObject);
                _placedTurrets.Remove(coords);

                if (DestroyTurretSound != null)
                    AudioManager.Instance.PlayClip(transform.position, DestroyTurretSound);
            }
        }

        private bool PlaceTurret(TurretInfo turret)
        {
            if (turret.Prefab == null)
            {
                Debug.LogWarning("No prefab for turret");
                return false;
            }

            var coords = PlacementGrid.Instance.WorldToCoords(transform.TransformPoint(ReferencePoint));
            var cellCenter = PlacementGrid.Instance.CenterOfCell(coords, TurretRadiusOffset);
            var placementPosition = cellCenter + new Vector3(0, TurretHeightOffset, 0);

            var turretObj = GameObject.Instantiate(turret.Prefab, placementPosition, PlacementGrid.Instance.RotationAlongTangent(placementPosition));
            var turretCom = turretObj.GetComponent<Turret>();
            if (turretCom != null)
            {
                _placedTurrets.Add(coords, turretCom);
                return true;
            }

            return false;
        }

        private void OnTurretInRange()
        {
            UITurretInfo.Instance.Show();
            UITurretInfo.Instance.Target = _currentTurret.gameObject;
        }

        private void OnTurretOutOfRange()
        {
            UITurretInfo.Instance.Hide();
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
