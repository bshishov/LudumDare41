using System;
using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Collider))]
    public class Zone : MonoBehaviour
    {
        [TagSelector]
        public string Target = Tags.Enemy;
        public float TTL = 10f;

        public bool DestroyOnHit = false;

        [SerializeField]
        public Effect[] HitEffects;

        [NonSerialized]
        public float DamageMultiplier = 1f;

        void Start ()
        {
            Destroy(gameObject, TTL);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Target))
            {
                var enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    foreach (var hitEffect in HitEffects)
                        enemy.ApplyEffect(hitEffect, DamageMultiplier);
                }

                if (DestroyOnHit)
                    Destroy(gameObject);
            }
        }
    }
}
