using System;
using Assets.Scripts.UI;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Collider))]
    public class SunClock : Singleton<MonoBehaviour>
    {
        public float Pool;

        [Header("UI")]
        public Text Text;
        public UIProgressBar ProgressBar;

        public float Current { get; private set; }

        public float Progress { get { return Mathf.Clamp01(1 - Current / Pool); } }
        
        void Start ()
        {
            UpdateAppearance();
        }
        
        void Update ()
        {
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tags.Enemy))
            {
                Current += 1;
                if (Current > Pool)
                {
                    UIGameOver.Instance.Show();
                    Current = Pool;
                }

                var enemy = other.gameObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.Die(dropsouls: false);
                }
                else
                {
                    Destroy(other.gameObject);
                }
                UpdateAppearance();
            }
        }

        void UpdateAppearance()
        {
            if (Text != null)
                Text.text = string.Format("Clock: {0}/{1}", Mathf.FloorToInt(Pool - Current), Mathf.FloorToInt(Pool));

            if (ProgressBar != null)
                ProgressBar.Value = Progress;
        }
    }
}
