using System;
using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.Sound;
using UnityEngine;

namespace Assets.Scripts.Turrets
{
    [RequireComponent(typeof(Aoe))]
    [RequireComponent(typeof(Buffable))]
    public class Turret : MonoBehaviour
    {
        public enum ActivationType
        {
            Automatic,
            Manual
        }

        public TurretInfo Info;

        [Header("Fire Mode")]
        public ActivationType FireMode;
        [Tooltip("Delay between firing. If burst > 1 then it is a delay between bursts")]
        public float Cooldown = 1f;

        [Tooltip("Delay between projectiles in a single burst")]
        public float BurstDelay = 0.1f;

        [Tooltip("Number of projectiles to emit in a single burst")]
        public int Burst = 1;


        public float CooldownPercentage { get { return Mathf.Clamp01(1 - _timeSinceLastActivation / Cooldown); } }
        public Vector2Int GridCoords { get; private set; }
        public Direction Direction { get { return _aoe.Direction; } }

        [Header("Audio")]
        public AudioClipWithVolume FireSound;

        public event Action OnFire;

        private float _timeSinceLastActivation = 0f;
        private float _burstCd = 0f;
        private int _burstsDone;
        private Buffable _buffable;
        
        private Aoe _aoe;

        void Awake()
        {
            _aoe = GetComponent<Aoe>();
            UpdateRotation();
        }

        void Start ()
        {
            GridCoords = PlacementGrid.Instance.WorldToCoords(transform.position);
            _timeSinceLastActivation = Cooldown + 1f;
            _buffable = GetComponent<Buffable>();
        }

        private void UpdateRotation()
        {
            if (Direction == Direction.Left)
                transform.rotation = PlacementGrid.Instance.RotationAlongTangent(transform.position, left: true);
            else if (Direction == Direction.Right)
                transform.rotation = PlacementGrid.Instance.RotationAlongTangent(transform.position, left: false);
            else if (Direction == Direction.None)
                transform.rotation = PlacementGrid.Instance.RotationAlongNormal(transform.position);
        }
	
        void Update ()
        {
            _timeSinceLastActivation += Time.deltaTime * _buffable.CooldownInvMultiplier;

            if (_aoe.ObjectsInAoE != null && _aoe.ObjectsInAoE.Count > 0)
            {
                if (FireMode == ActivationType.Automatic && _timeSinceLastActivation > Cooldown)
                {
                    _burstCd += Time.deltaTime * _buffable.CooldownInvMultiplier;
                    if (_burstCd > BurstDelay)
                    {
                        _burstsDone++;
                        _burstCd = 0f;

                        if (OnFire != null)
                        {
                            OnFire();
                            if(FireSound != null)
                                AudioManager.Instance.PlayClip(transform.position, FireSound);
                        }

                        if (_burstsDone >= Burst)
                        {
                            _burstsDone = 0;
                            _burstCd = 0;
                            _timeSinceLastActivation = 0;
                        }
                    }
                }
            }
            else
            {
                //_timeSinceLastActivation = Cooldown + 1f;
                _burstCd = BurstDelay + 1f;
                _burstsDone = 0;
            }
        }

        [ContextMenu("Fire!")]
        public void Activate()
        {
            if(FireMode != ActivationType.Manual)
                return;

            if (_timeSinceLastActivation > Cooldown)
            {
                if (OnFire != null)
                    OnFire();

                if (FireSound != null)
                    AudioManager.Instance.PlayClip(transform.position, FireSound);

                _timeSinceLastActivation = 0;
            }
        }

        [ContextMenu("Change Direction")]
        public void ChangeDirection()
        {
            if (Direction == Direction.None)
            {
                Debug.LogWarning("Trying to change direction of non Directional aoe type");
                return;
            }

            if (Direction == Direction.Left)
                _aoe.Direction = Direction.Right;
            else if (Direction == Direction.Right)
                _aoe.Direction = Direction.Left;

            // ROTATE FIRST
            UpdateRotation();

            // REBUILD COLLIDERS NEXT
            _aoe.RebuildAoe();
        }
    }
}
