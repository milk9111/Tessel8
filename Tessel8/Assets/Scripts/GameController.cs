using System.Collections.Generic;
using System.Linq;
using EnemyControllers;
using Spawn;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameController : MonoBehaviour
    {
        public PauseMenu pauseMenu;

        public GameoverMenu gameOverMenu;

        public GameoverMenu victoryMenu;
        
        private EnemySpawnController _spawnController;
        private SpawnUIController _spawnUiController;

        private GameObject _player;
        private PlayerControllerHelper _helper;
        private Vector3 _playerStartingPosition;
        private Quaternion _playerStartingRotation;

        private IList<GameObject> _interactiveTiles;
        
        void Awake()
        {
            _spawnController = GetComponent<EnemySpawnController>();
            _spawnUiController = GetComponent<SpawnUIController>();
            _player = GameObject.FindWithTag("Player");
            _helper = _player.GetComponent<PlayerControllerHelper>();
            _playerStartingPosition = new Vector3(_player.transform.position.x, 
                _player.transform.position.y, _player.transform.position.z);
            _playerStartingRotation = new Quaternion(_player.transform.rotation.x, 
                _player.transform.rotation.y, _player.transform.rotation.z, _player.transform.rotation.w);

            _interactiveTiles = GameObject.FindGameObjectsWithTag("InteractiveTile");
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

        public void OpenVictoryMenu()
        {
            victoryMenu.gameObject.SetActive(true);
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
            foreach (var tile in _interactiveTiles)
            {
                tile.GetComponent<FallingTile>().OnPause();
            }
        }

        public void ResumeGame()
        {
            pauseMenu.gameObject.SetActive(false);
            _helper.OnPlay();
            _spawnController.OnPlay();
            foreach (var tile in _interactiveTiles)
            {
                tile.GetComponent<FallingTile>().OnPlay();
            }
        }

        public void StartGame()
        {
            gameOverMenu.gameObject.SetActive(false);
            pauseMenu.gameObject.SetActive(false);
            victoryMenu.gameObject.SetActive(false);
            _spawnController.OnStart();
            _spawnUiController.OnStart();
            _helper.OnStart(_playerStartingPosition, _playerStartingRotation);
            foreach (var tile in _interactiveTiles)
            {
                tile.GetComponent<FallingTile>().OnStart();
            }
        }

        public void GameOver()
        {
            if (victoryMenu.gameObject.activeInHierarchy) return;
            gameOverMenu.gameObject.SetActive(true);
        }
    }
}