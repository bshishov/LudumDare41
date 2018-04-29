using UnityEngine;

namespace Assets.Scripts.Enemies
{
    [RequireComponent(typeof(Enemy))]
    public class HealSelf : MonoBehaviour
    {
        public float Cooldown = 1f;
        public float Amount = 1f;

        [Header("Animation")]
        public string TriggerAnimation;

        private Enemy _self;
        private float _t;
        private Animator _animator;

        void Start ()
        {
            _t = Cooldown;
            _self = GetComponent<Enemy>();
            _animator = GetComponent<Animator>();
        }
	
        void Update ()
        {
            _t += Time.deltaTime;
            if (_t > Cooldown)
            {
                if (_self.CurrentHp < _self.MaxHp)
                {
                    _self.Heal(Amount);
                    _t = 0;

                    if (_animator != null && !string.IsNullOrEmpty(TriggerAnimation))
                    {
                        _animator.SetTrigger(TriggerAnimation);
                    }
                }
            }
        }
    }
}
