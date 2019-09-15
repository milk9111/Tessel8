using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UserInterface
{
    public class ArenaSelection : MonoBehaviour
    {
        public GameObject startButton;
        public Text highScoreText;

        public HighScore highScore;
        
        private ArenaOption _currSelected;

        void Awake()
        {
            startButton.SetActive(false);
            highScoreText.enabled = false;
        }

        public void OnNewSelection(ArenaOption newSelected)
        {
            if (_currSelected != null) _currSelected.Deselect();
            _currSelected = newSelected;
            startButton.SetActive(true);
            highScoreText.enabled = true;
            highScoreText.text = "High Score: " + highScore.GetScoreForArena(_currSelected.arenaName);
        }

        public void LoadArena()
        {
            Debug.Log("Loading arena " + _currSelected.arenaName);
            SceneManager.LoadScene(_currSelected.arenaName);
        }
    }
}