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

        private Buffable _selfBuffable;
        private List<Buffable> _aoeBuffables = new List<Buffable>();
        private float _t;
        
        void Start()
        {
            _selfBuffable = GetComponent<Buffable>();
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
