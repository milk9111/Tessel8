using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class PauseMenu : MonoBehaviour
    {
        public GameController gameController;

        private bool _toggleMusic = true;
        private bool _toggleSoundFx = true;

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

        public void ToggleMusic()
        {
            _toggleMusic = !_toggleMusic;
            gameController.ToggleMusic();
        }

        public void ToggleSoundFx()
        {
            _toggleSoundFx = !_toggleSoundFx;
            gameController.ToggleSoundFx();
        }
    }
}