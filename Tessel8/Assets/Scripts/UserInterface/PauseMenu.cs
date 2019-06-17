using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class PauseMenu : MonoBehaviour
    {
        public GameController gameController;

        public void OnPause()
        {
            gameController.PauseGame();
        }

        public void OnPlay()
        {
            gameController.ResumeGame();
        }

        public void OnExit()
        {
            SceneManager.LoadScene("start_screen");
        }
    }
}