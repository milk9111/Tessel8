using UnityEngine;
using UnityEngine.SceneManagement;

namespace UserInterface
{
    public class MainMenu : MonoBehaviour
    {
        public void OnStart()
        {
            SceneManager.LoadScene("tiles_main");
        }
        
        public void OnExit()
        {
            Application.Quit();
        }
    }
}