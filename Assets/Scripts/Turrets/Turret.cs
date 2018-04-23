using System;
using System.Linq;
using Assets.Scripts.Data;
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

        [Header("Fire Mode")] public ActivationType FireMode;
        [Tooltip("Delay between firing. If burst > 1 then it is a delay between bursts")]
        public float Cooldown = 1f;

        [Tooltip("Delay between projectiles in a single burst")]
        public float BurstDelay = 0.1f;

        [Tooltip("Number of projectiles to emit in a single burst")]
        public int Burst = 1;


        public float CooldownPercentage { get { return Mathf.Clamp01(1 - _cd / Cooldown); } }
        public Vector2Int GridCoords { get; private set; }
        public Direction Direction { get { return _aoe.Direction; } }

        public event Action OnFire;

        private float _cd = 0f;
        private float _burstCd = 0f;
        private int _burstsDone;
        private Buffable _buffable;
        
        private Aoe _aoe;
        
        void Start ()
        {
            GridCoords = PlacementGrid.Instance.WorldToCoords(transform.position);
            _cd = Cooldown + 1f;
            _buffable = GetComponent<Buffable>();
            _aoe = GetComponent<Aoe>();
        }
	
        void Update ()
        {
            _cd += Time.deltaTime * _buffable.CooldownInvMultiplier;

            if (_aoe.ObjectsInAoE != null && _aoe.ObjectsInAoE.Count > 0)
            {
                if (FireMode == ActivationType.Automatic && _cd > Cooldown)
                {
                    _burstCd += Time.deltaTime * _buffable.CooldownInvMultiplier;
                    if (_burstCd > BurstDelay)
                    {
                        _burstsDone++;
                        _burstCd = 0f;

                        if (OnFire != null)
                            OnFire();

                        if (_burstsDone >= Burst)
                        {
                            _burstsDone = 0;
                            _burstCd = 0;
                            _cd = 0;
                        }
                    }
                }
            }
            else
            {
                //_cd = Cooldown + 1f;
                _burstCd = BurstDelay + 1f;
                _burstsDone = 0;
            }
        }

        [ContextMenu("Fire!")]
        public void Activate()
        {
            if(FireMode != ActivationType.Manual)
                return;

            if (_cd > Cooldown)
            {
                if (OnFire != null)
                    OnFire();

                _cd = 0;
            }
        }

        [ContextMenu("Change Direction")]
        public void ChangeDirection()
        {
            if (_aoe.AoEType != AoEType.HorizontalDirectionalPlatform)
            {
                Debug.LogWarning("Trying to change direction of non Directional aoe type");
                return;
            }

            if (Direction == Direction.Left)
                _aoe.Direction = Direction.Right;
            else if (Direction == Direction.Right)
                _aoe.Direction = Direction.Left;

            _aoe.RebuildAoe();
        }
    }
}
