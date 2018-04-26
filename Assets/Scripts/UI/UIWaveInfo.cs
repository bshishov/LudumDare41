using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UIWaveInfo : MonoBehaviour
    {
        public Text WaveName;
        public Text EnemiesCount;
        public RectTransform ProgressBar;

        private WaveManager _waveManager;
        private Vector2 _progressBarSize;

        void Start ()
        {
            _waveManager = WaveManager.Instance;
            if (ProgressBar != null)
                _progressBarSize = ProgressBar.sizeDelta;
        }
	
        void Update ()
        {
            if (_waveManager != null)
            {
                if (_waveManager.WaveInProgress)
                {
                    if (WaveName != null)
                        WaveName.text = string.Format("Wave {0}", _waveManager.WaveNumber);

                    if (EnemiesCount != null)
                        EnemiesCount.text = string.Format("{0}/{1}", _waveManager.EnemiesOut,
                            _waveManager.CurrentWave.TotalNumberOfEnemies());

                    ProgressBar.sizeDelta =
                        new Vector2(_progressBarSize.x * _waveManager.Percentage, _progressBarSize.y);
                }
                else
                {
                    if (WaveName != null)
                        WaveName.text = "Next wave in";

                    if (EnemiesCount != null)
                        EnemiesCount.text = string.Format("{0} seconds", Mathf.FloorToInt(_waveManager.TimeToNextWave));

                    ProgressBar.sizeDelta = new Vector2(_progressBarSize.x * 0, _progressBarSize.y);
                }
            }
        }
    }
}
