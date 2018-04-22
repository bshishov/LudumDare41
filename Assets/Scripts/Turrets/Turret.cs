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
        public TowerInfo Info;

        [Header("Fire Mode")]
        [Tooltip("Delay between firing. If burst > 1 then it is a delay between bursts")]
        public float Cooldown = 1f;

        [Tooltip("Delay between projectiles in a single burst")]
        public float BurstDelay = 0.1f;

        [Tooltip("Number of projectiles to emit in a single burst")]
        public int Burst = 1;

        public Vector2Int GridCoords { get; private set; }
        public Direction Direction { get; private set; }

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
            if (_aoe.ObjectsInAoE != null && _aoe.ObjectsInAoE.Count > 0)
            {
                _cd += Time.deltaTime * _buffable.CooldownInvMultiplier;
                if (_cd > Cooldown)
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
                _cd = Cooldown + 1f;
                _burstCd = BurstDelay + 1f;
                _burstsDone = 0;
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
            {
                Direction = Direction.Right;
            }
            else if (Direction == Direction.Right)
            {
                Direction = Direction.Left;
            }
        }
    }
}
