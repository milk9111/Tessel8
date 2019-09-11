using UnityEngine;
using UnityEngine.SceneManagement;

namespace UserInterface
{
    public class MainMenu : MonoBehaviour
    {
        public GameObject startScreen;
        public GameObject arenaSelection;
        public Credits credits;

        void Awake()
        {
            OnBack();
        }
        
        public void OnStart()
        {
            startScreen.SetActive(false);
            arenaSelection.SetActive(true);
            credits.gameObject.SetActive(false);
        }

        public void OnCredits()
        {
            startScreen.SetActive(false);
            arenaSelection.SetActive(false);
            credits.gameObject.SetActive(true);
            credits.StartCredits();
        }

        public void OnBack()
        {
            startScreen.SetActive(true);
            arenaSelection.SetActive(false);
            credits.gameObject.SetActive(false);
        }
        
        public void OnExit()
        {
            Application.Quit();
        }
    }
}