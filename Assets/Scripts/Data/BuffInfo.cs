using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(fileName = "buff", menuName = "Assets/GamePlay/Buff")]
    public class BuffInfo : ScriptableObject
    {
        public string Name;
        [Tooltip("Duration in seconds")]
        public float Duration = 1f;
        public float TickRate = 0.1f;

        [Header("Modifiers")]
        public float DamageModifier = 0f;
        public float CooldownModifier = 0f;
        public float SpeedModifier = 0f;

        [Header("NOT USED")]
        public float HpModifier = 0f;

        [Header("Effects")]
        [SerializeField]
        public Effect[] OnApplyEffects;

        [SerializeField]
        public Effect[] OnTickEffects;

        [SerializeField]
        public Effect[] OnRemoveEffects;
    }
}
