using UnityEngine;

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
            Application.Quit();
        }
    }
}