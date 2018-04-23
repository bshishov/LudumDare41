using Assets.Scripts.Data;
using Assets.Scripts.UI;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Buffable))]
    public class Enemy : MonoBehaviour
    {
        public float MaxHp = 10;
        public float CurrentHp { get; private set; }

        private Buffable _buffable;
	
        void Start ()
        {
            _buffable = GetComponent<Buffable>();
            _buffable.OnApplyEffect += ApplyEffect;

            CurrentHp = MaxHp;
        }

        void Update()
        {
        }

        public void TakeDamage(float amount)
        {
            CurrentHp -= amount;
            if (CurrentHp < 1f)
                Destroy(gameObject);

            UINotifications.Instance.Show(gameObject.transform, amount.ToString(), Color.red);
        }

        public void Heal(float amount)
        {
            CurrentHp = Mathf.Max(CurrentHp + amount, MaxHp);

            UINotifications.Instance.Show(gameObject.transform, amount.ToString(), Color.green);
        }

        public void ApplyEffect(Effect effect)
        {
            if (effect.DealDamage > 0)
            {
                TakeDamage(effect.DealDamage);
            }

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

        void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                Handles.Label(transform.position + Vector3.down, string.Format("HP:{0}/{1}", CurrentHp, MaxHp), "textField");
            }
        }
    }
}
