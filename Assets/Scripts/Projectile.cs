using System;
using System.Collections.Generic;
using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class Projectile : MonoBehaviour
    {
        [TagSelector]
        public string Target = Tags.Enemy;
        public float TTL = 10f;
        public bool Piercing = false;

        [SerializeField]
        public List<Effect> HitEffects;

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

                if (!Piercing)
                    Destroy(gameObject);
            }
        }
    }
}
