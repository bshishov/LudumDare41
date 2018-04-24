using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data;
using UnityEngine;

#if DEBUG
using UnityEditor;
#endif

namespace Assets.Scripts
{
    public class Buffable : MonoBehaviour
    {
        class BuffState
        {
            public BuffInfo Buff;
            public float TimeRemaining;
            public float TickCd;
        }

        private readonly List<BuffState> _states = new List<BuffState>();
        private readonly List<BuffState> _statesToRemove = new List<BuffState>();

        public event Action<Effect> OnApplyEffect;
        public float CooldownModifier { get; private set; }
        public float DamageModifier { get; private set; }
        public float SpeedModifier { get; private set; }
        public float HpModifier { get; private set; }
        public float CooldownMultiplier { get { return 1 + CooldownModifier; } }
        public float CooldownInvMultiplier { get { return 1 / CooldownMultiplier; } }
        public float DamageMultiplier { get { return 1 + DamageModifier; } }
        public float SpeedMultiplier { get { return 1 + SpeedModifier; } }
        public float HpMultiplier { get { return 1 + HpModifier; } }

        void Start ()
        {
        }
        
        void Update ()
        {
            foreach (var buffState in _states)
            {
                buffState.TickCd -= Time.deltaTime;
                buffState.TimeRemaining -= Time.deltaTime;

                if (buffState.TickCd < 0)
                {
                    ApplyEffects(buffState.Buff.OnTickEffects);
                    buffState.TickCd = buffState.Buff.TickRate;
                }

                if (buffState.TimeRemaining < 0)
                    _statesToRemove.Add(buffState);
            }

            foreach (var buffState in _statesToRemove)
            {
                CooldownModifier -= buffState.Buff.CooldownModifier;
                DamageModifier -= buffState.Buff.DamageModifier;
                SpeedModifier -= buffState.Buff.SpeedModifier;
                HpModifier -= buffState.Buff.HpModifier;

                ApplyEffects(buffState.Buff.OnRemoveEffects);
                _states.Remove(buffState);
            }

            _statesToRemove.Clear();
        }

        public void AddBuff(BuffInfo buff, float durationOverride = 0f)
        {
            var duration = buff.Duration;
            if (durationOverride > 0)
                duration = durationOverride;

            var existingBuff = _states.FirstOrDefault(s => s.Buff == buff);
            // If no such buff already applied - apply new
            if (existingBuff == null)
            {
                _states.Add(new BuffState
                {
                    Buff = buff,
                    TickCd = buff.TickRate,
                    TimeRemaining = duration
                });

                CooldownModifier += buff.CooldownModifier;
                DamageModifier += buff.DamageModifier;
                SpeedModifier += buff.SpeedModifier;
                HpModifier += buff.HpModifier;

                ApplyEffects(buff.OnApplyEffects);
            }
            else
            {
                // Refresh existing buff
                existingBuff.TickCd = buff.TickRate;
                existingBuff.TimeRemaining = duration;
            }
        }

        private void ApplyEffects(IEnumerable<Effect> effects)
        {
            if (OnApplyEffect == null)
                return;

            foreach (var effect in effects)
            {
                OnApplyEffect.Invoke(effect);
            }
        }

        void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                var buffsDesc = string.Join("\n", _states.Select(s => string.Format("{0}: {1:0.0}", s.Buff.Name, s.TimeRemaining)).ToArray());
                var stateDesc = string.Format("SPD: {0:0.0}\nDMG: {1:0.0}\nCDR: {2:0.0}\nHPB: {3:0.0}", SpeedMultiplier, DamageMultiplier, CooldownMultiplier, HpMultiplier);

#if DEBUG
                Handles.Label(transform.position + Vector3.down * 2, string.Format("Buffs:\n{0}\n{1}", buffsDesc, stateDesc), "textField");
#endif
            }
        }
    }
}
