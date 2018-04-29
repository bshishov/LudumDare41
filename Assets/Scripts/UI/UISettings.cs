using Assets.Scripts.Sound;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(UICanvasGroupFader))]
    public class UISettings : Singleton<UIGameOver>
    {
        public GameObject StartFocus;
        public Text MusicVolume;
        public Text SoundVolume;

        private UIBuildingMenu _buildingMenu;
        private UICanvasGroupFader _fader;

        void Start ()
        {
            _buildingMenu = (UIBuildingMenu)FindObjectOfType(typeof(UIBuildingMenu));
            _fader = GetComponent<UICanvasGroupFader>();
        }

        void Update ()
        {
            if (Input.GetButtonDown("Menu"))
            {
                if (_buildingMenu != null)
                {
                    if (_buildingMenu.IsActive)
                        _buildingMenu.Hide();
                }

                if (_fader.State == UICanvasGroupFader.FaderState.FadedOut)
                    Show();

                if (_fader.State == UICanvasGroupFader.FaderState.FadedIn)
                    Hide();

            }

            if (_fader.State == UICanvasGroupFader.FaderState.FadingIn ||
                _fader.State == UICanvasGroupFader.FaderState.FadedIn)
            {
                if (MusicVolume != null)
                    MusicVolume.text = string.Format("Music\n{0:##0}%", AudioManager.Instance.MusicVolume * 100f);

                if (SoundVolume != null)
                    SoundVolume.text = string.Format("Sound\n{0:##0}%", AudioManager.Instance.SoundsVolume * 100f);
            }
        }

        public void Show()
        {
            if (StartFocus != null)
                EventSystem.current.SetSelectedGameObject(StartFocus.gameObject);

            Time.timeScale = 0f;

            if(_buildingMenu != null)
                if (_buildingMenu.IsActive)
                    _buildingMenu.Hide();

            _fader.FadeIn();
        }

        public void Hide()
        {
            Time.timeScale = 1f;
            _fader.FadeOut();
        }

        public void LoadScene(string scene)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(scene);
        }

        public void ChangeMusicVolume(float amount)
        {
            AudioManager.Instance.MusicVolume = Mathf.Clamp01(AudioManager.Instance.MusicVolume + amount);
        }

        public void ChangeSoundVolume(float amount)
        {
            AudioManager.Instance.SoundsVolume = Mathf.Clamp01(AudioManager.Instance.SoundsVolume + amount);
        }
    }
}
