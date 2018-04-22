using System;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class Effect
    {
        public float DealDamage = 0f;
        public float Heal = 0f;
        public BuffInfo ApplyBuff;

        [Header("Spawn")]
        public GameObject SpawnObject;
        public bool WorldSpace = true;

        [Header("Misc")]
        public AudioClip PlaySound;
    }
}
