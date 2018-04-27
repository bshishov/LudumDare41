using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(UICanvasGroupFader))]
    public class UIGameOver : Singleton<UIGameOver>
    {
        private UICanvasGroupFader _fader;

        void Start()
        {
            _fader = GetComponent<UICanvasGroupFader>();
        }

        public void LoadScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }

        public void Show()
        {
            Time.timeScale = 0f;
            _fader.FadeIn();
        }

        public void Hide()
        {
            Time.timeScale = 1f;
            _fader.FadeOut();
        }
    }
}
