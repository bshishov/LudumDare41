﻿using Assets.Scripts.Data;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.AI;
#if DEBUG
using UnityEditor;
#endif

namespace Assets.Scripts
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Buffable))]
    public class Enemy : MonoBehaviour
    {
        public float BaseSpeed = 3.5f;
        public float LadderSpeed = 0.5f;
        public float MaxHp = 10;
        public int DropSouls = 1;
        public GameObject SoulPrefab;
        public float CurrentHp { get; private set; }

        private Vector3 _target;
        private Buffable _buffable;
        private NavMeshAgent _agent;
        private Animator _animator;

        void Awake()
        {
            _buffable = GetComponent<Buffable>();
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();
        }

        void Start ()
        {
            _buffable.OnApplyEffect += ApplyEffect;
            CurrentHp = MaxHp;
        }

        public void SetTarget(Vector3 target)
        {
            _target = target;
            _agent.SetDestination(target);
        }

        void Update()
        {
            if(_agent == null)
                return;

            if (_agent.isOnOffMeshLink)
            {
                _agent.speed = LadderSpeed * _buffable.SpeedMultiplier;

                if (_animator != null)
                    _animator.SetBool("IsClimbing", true);
            }
            else
            {
                _agent.speed = BaseSpeed * _buffable.SpeedMultiplier;

                if (_animator != null)
                    _animator.SetBool("IsClimbing", false);
            }
        }

        public void TakeDamage(float amount)
        {
            UINotifications.Instance.Show(gameObject.transform, amount.ToString(), Color.red, yOffset: 2f);

            CurrentHp -= amount;
            if (CurrentHp < 1f)
                Die();
        }

        public void Heal(float amount)
        {
            CurrentHp = Mathf.Max(CurrentHp + amount, MaxHp);

            UINotifications.Instance.Show(gameObject.transform, amount.ToString(), Color.green, yOffset: 2f);
        }

        public void ApplyEffect(Effect effect)
        {
            if (effect.DealDamage > 0)
                TakeDamage(effect.DealDamage);

            if (effect.Heal > 0)
                Heal(effect.Heal);

            if (effect.ApplyBuff != null)
                _buffable.AddBuff(effect.ApplyBuff);

            if (effect.SpawnObject != null)
            {
                if (effect.WorldSpace)
                    GameObject.Instantiate(effect.SpawnObject, transform.position, Quaternion.identity);
                else
                    GameObject.Instantiate(effect.SpawnObject, transform);
            }
        }

        public void Die()
        {
            if (SoulPrefab != null)
            {
                var soulObj = GameObject.Instantiate(SoulPrefab, transform.position, Quaternion.identity);
                var soul = soulObj.GetComponent<Soul>();
                if(soul != null)
                    soul.Value = DropSouls;
            }
            Destroy(gameObject);
        }

        void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
#if DEBUG
                Handles.Label(transform.position + Vector3.down, string.Format("HP:{0}/{1}", CurrentHp, MaxHp), "textField");
#endif
            }
        }
    }
}
