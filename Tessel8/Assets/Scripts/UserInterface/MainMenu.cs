using UnityEngine;
using UnityEngine.SceneManagement;

namespace UserInterface
{
    public class MainMenu : MonoBehaviour
    {
        public GameObject startScreen;
        public GameObject arenaSelection;

        void Awake()
        {
            OnBack();
        }
        
        public void OnStart()
        {
            startScreen.SetActive(false);
            arenaSelection.SetActive(true);
        }

        public void OnBack()
        {
            startScreen.SetActive(true);
            arenaSelection.SetActive(false);
        }
        
        public void OnExit()
        {
            Application.Quit();
        }
    }
}