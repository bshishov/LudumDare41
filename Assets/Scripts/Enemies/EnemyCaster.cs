using System.Collections.Generic;
using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(Buffable))]
    [RequireComponent(typeof(Collider))]
    public class EnemyCaster : MonoBehaviour
    {
        public float CoolDown = 1f;
        public BuffInfo Buff;
        public bool BuffSelf = false;

        [TagSelector]
        public string Target;

        [Header("Animation")]
        public string TriggerAnimation;

        private Buffable _selfBuffable;
        private List<Buffable> _aoeBuffables = new List<Buffable>();
        private float _t;
        private Animator _animator;

        void Start()
        {
            _t = CoolDown;
            _selfBuffable = GetComponent<Buffable>();
            _animator = GetComponent<Animator>();
        }
        
        void Update()
        {
            _t += Time.deltaTime * _selfBuffable.CooldownInvMultiplier;
            if (_t > CoolDown)
            {
                if (_aoeBuffables.Count == 0 && !BuffSelf)
                    return;
                
                ApplyBuff();
                _t = 0;

                if (_animator != null && !string.IsNullOrEmpty(TriggerAnimation))
                    _animator.SetTrigger(TriggerAnimation);
            }
        }

        void ApplyBuff()
        {
            foreach (var buffable in _aoeBuffables)
                buffable.AddBuff(Buff);

            if (BuffSelf)
                Debug.LogFormat("Buffing self");
                _selfBuffable.AddBuff(Buff);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Target))
            {
                var buffable = other.GetComponent<Buffable>();
                if (buffable != null)
                {
                    _aoeBuffables.Add(buffable);
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Target))
            {
                var buffable = other.GetComponent<Buffable>();
                if (buffable != null && _aoeBuffables.Contains(buffable))
                    _aoeBuffables.Remove(buffable);
            }
        }
    }
}
