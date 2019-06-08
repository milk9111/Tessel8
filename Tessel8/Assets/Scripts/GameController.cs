using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameController : MonoBehaviour
    {
        public PauseMenu pauseMenu;
        
        private EnemySpawnController _spawnController;

        private GameObject _player;
        private PlayerControllerHelper _helper;
        
        void Awake()
        {
            _spawnController = GetComponent<EnemySpawnController>();
            _player = GameObject.FindWithTag("Player");
            _helper = _player.GetComponent<PlayerControllerHelper>();
        }

        public IList<EnemyController> GetAllEnemiesInGame()
        {
            return _spawnController.GetAllEnemiesInGame();
        }

        public void OpenPauseMenu()
        {
            pauseMenu.gameObject.SetActive(true);
            pauseMenu.OnPause();
        }
        
        public void ClosePauseMenu()
        {
            pauseMenu.OnPlay();
        }

        public void PauseGame()
        {
            pauseMenu.gameObject.SetActive(true);
            _helper.OnPause();
            _spawnController.OnPause();
        }

        public void ResumeGame()
        {
            pauseMenu.gameObject.SetActive(false);
            _helper.OnPlay();
            _spawnController.OnPlay();
        }
    }
}