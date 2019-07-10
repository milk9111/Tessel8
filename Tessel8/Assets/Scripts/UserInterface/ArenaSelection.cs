using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UserInterface
{
    public class ArenaSelection : MonoBehaviour
    {
        public GameObject startButton;
        
        private ArenaOption _currSelected;

        void Awake()
        {
            startButton.SetActive(false);
        }

        public void OnNewSelection(ArenaOption newSelected)
        {
            if (_currSelected != null) _currSelected.Deselect();
            _currSelected = newSelected;
            startButton.SetActive(true);
        }

        public void LoadArena()
        {
            Debug.Log("Loading arena " + _currSelected.arenaName);
            SceneManager.LoadScene(_currSelected.arenaName);
        }
    }
}