using Assets.Scripts.Data;
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
        
        private Buffable _buffable;
        private NavMeshAgent _agent;
        private Animator _animator;

        private Renderer _renderer;

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
            _renderer = GetComponent<Renderer>();
            if (_renderer == null)
                _renderer = GetComponentInChildren<Renderer>();

            _buffable.OnApplyEffect += ApplyEffect;
            CurrentHp = MaxHp;
        }

        private void ApplyEffect(Effect obj)
        {
            ApplyEffect(obj, 1f);
        }

        public void SetTarget(Vector3 target)
        {
            _agent.SetDestination(target);
        }

        void Update()
        {
            if(_agent == null)
                return;

            _animator.SetFloat("Speed", _buffable.SpeedMultiplier);
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
            if(_renderer.isVisible)
                UINotifications.Instance.Show(gameObject.transform, amount.ToString(), Color.red, yOffset: 2f);

            if (CurrentHp > 0)
            {
                CurrentHp -= amount;
                if (CurrentHp < 1f)
                    Die();
            }
        }

        public void Heal(float amount)
        {
            CurrentHp = Mathf.Max(CurrentHp + amount, MaxHp);

            if (_renderer.isVisible)
                UINotifications.Instance.Show(gameObject.transform, amount.ToString(), Color.green, yOffset: 2f);
        }

        public void ApplyEffect(Effect effect, float damageMultiplier = 1f)
        {
            if (effect.DealDamage > 0)
                TakeDamage(effect.DealDamage * damageMultiplier);

            if (effect.Heal > 0)
                Heal(effect.Heal);

            if (effect.ApplyBuff != null)
                _buffable.AddBuff(effect.ApplyBuff);

            if (effect.SpawnObject != null)
            {
                if (effect.WorldSpace)
                    GameObject.Instantiate(effect.SpawnObject, transform.position, Quaternion.identity);
                else
                    GameObject.Instantiate(effect.SpawnObject, transform, false);
            }
        }

        public void Die(bool dropsouls = true)
        {
            if (dropsouls)
            {
                if (SoulPrefab != null)
                {
                    var soulObj = GameObject.Instantiate(SoulPrefab, transform.position, Quaternion.identity);
                    var soul = soulObj.GetComponent<Soul>();
                    if (soul != null)
                        soul.Value = DropSouls;
                }
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
