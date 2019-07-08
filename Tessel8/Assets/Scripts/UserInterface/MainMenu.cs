using UnityEngine;
using UnityEngine.SceneManagement;

namespace UserInterface
{
    public class MainMenu : MonoBehaviour
    {
        public void OnStart(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        
        public void OnExit()
        {
            Application.Quit();
        }
    }
}