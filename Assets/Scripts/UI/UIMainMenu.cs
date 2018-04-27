using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI
{
    public class UIMainMenu : MonoBehaviour
    {
        public void Play()
        {
            SceneManager.LoadScene("main");
        }

        public void DontPlay()
        {
            Application.Quit();
        }
    }
}
