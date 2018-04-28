using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    [RequireComponent(typeof(Enemy))]
    public class DPSCounter : MonoBehaviour
    {
        private Enemy _enemy;
        private float _t;
        private float _lastHp;
        
        void Start ()
        {
            _enemy = GetComponent<Enemy>();
            _lastHp = _enemy.CurrentHp;
        }
	
        
        void Update ()
        {
            _t += Time.deltaTime;
            if (_t > 1f)
            {
                var dps = _lastHp - _enemy.CurrentHp;
                _lastHp = _enemy.CurrentHp;

                if (dps > 0)
                {
                    UINotifications.Instance.Show(transform, string.Format("DPS: {0:#}", dps), Color.white, yOffset: 2f);
                }
                _t = 0f;
            }
        }
    }
}
