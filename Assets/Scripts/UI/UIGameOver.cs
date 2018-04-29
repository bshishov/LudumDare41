using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(UICanvasGroupFader))]
    public class UIGameOver : Singleton<UIGameOver>
    {
        public Button RestartButton;
        private UICanvasGroupFader _fader;

        void Start()
        {
            _fader = GetComponent<UICanvasGroupFader>();
        }

        public void LoadScene(string scene)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(scene);
        }

        public void Show()
        {
            if(RestartButton != null)
                EventSystem.current.SetSelectedGameObject(RestartButton.gameObject);
            Time.timeScale = 0f;

            if(UIBuildingMenu.Instance.IsActive)
                UIBuildingMenu.Instance.Hide();

            _fader.FadeIn();
        }

        public void Hide()
        {
            Time.timeScale = 1f;
            _fader.FadeOut();
        }
    }
}
