using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using EnemyControllers;
using Spawn;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UserInterface;

namespace DefaultNamespace
{
    public class GameController : MonoBehaviour
    {
        public PauseMenu pauseMenu;
        public GameoverMenu gameOverMenu;
        public GameoverMenu victoryMenu;
        public Text yourScoreText;
        public Text highScoreText;
        
        private EnemySpawnController _spawnController;
        private SpawnUIController _spawnUiController;
        private BackgroundMusicController _backgroundMusicController;
        private HighScore _highScore;

        private GameObject _player;
        private PlayerControllerHelper _helper;
        private Vector3 _playerStartingPosition;
        private Quaternion _playerStartingRotation;

        private IList<GameObject> _interactiveTiles;
        private IList<GameObject> _parallaxScrollingObjects;
        private IList<Vector3> _parallaxScrollingOriginalPositions;

        private AudioManager _audioManager;
        
        void Awake()
        {
            _spawnController = GetComponent<EnemySpawnController>();
            _spawnUiController = GetComponent<SpawnUIController>();
            _backgroundMusicController = GetComponent<BackgroundMusicController>();
            _highScore = GetComponent<HighScore>();
            _highScore.ResetScore();
            yourScoreText.text = "Your Score: " + _highScore.GetCurrentScore();
            highScoreText.text = "High Score: " + _highScore.GetScoreForArena(SceneManager.GetActiveScene().name);
            
            _player = GameObject.FindWithTag("Player");
            _helper = _player.GetComponent<PlayerControllerHelper>();
            _playerStartingPosition = new Vector3(_player.transform.position.x, 
                _player.transform.position.y, _player.transform.position.z);
            _playerStartingRotation = new Quaternion(_player.transform.rotation.x, 
                _player.transform.rotation.y, _player.transform.rotation.z, _player.transform.rotation.w);

            _interactiveTiles = GameObject.FindGameObjectsWithTag("InteractiveTile");
            _parallaxScrollingObjects = GameObject.FindGameObjectsWithTag("Parallax");
            _parallaxScrollingOriginalPositions = _parallaxScrollingObjects.Select(p => p.transform.position).ToList();
            _audioManager = FindObjectOfType<AudioManager>();
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

        public void ToggleMusic()
        {
            _backgroundMusicController.ToggleMusic();
        }

        public void ToggleSoundFx()
        {
            _audioManager.ToggleSoundFx();
        }

        public void PauseGame()
        {
            pauseMenu.gameObject.SetActive(true);
            _helper.OnPause();
            _spawnController.OnPause();
            _spawnUiController.OnPause();
            foreach (var tile in _interactiveTiles)
            {
                tile.GetComponent<FallingTile>().OnPause();
            }

            foreach (var scroll in _parallaxScrollingObjects)
            {
                scroll.GetComponent<ParallaxScrolling>().OnPause();
            }
        }

        public void ResumeGame()
        {
            pauseMenu.gameObject.SetActive(false);
            _helper.OnPlay();
            _spawnController.OnPlay();
            _spawnUiController.OnPlay();
            foreach (var tile in _interactiveTiles)
            {
                tile.GetComponent<FallingTile>().OnPlay();
            }
            
            foreach (var scroll in _parallaxScrollingObjects)
            {
                scroll.GetComponent<ParallaxScrolling>().OnPause();
            }
        }

        public void StartGame()
        {
            _highScore.ResetScore();
            yourScoreText.text = "Your Score: " + _highScore.GetCurrentScore();
            highScoreText.text = "High Score: " + _highScore.GetScoreForArena(SceneManager.GetActiveScene().name);
            
            gameOverMenu.gameObject.SetActive(false);
            pauseMenu.gameObject.SetActive(false);
            victoryMenu.gameObject.SetActive(false);
            _spawnController.OnStart();
            _spawnUiController.OnStart();
            _backgroundMusicController.OnStart();
            _helper.OnStart(_playerStartingPosition, _playerStartingRotation);
            foreach (var tile in _interactiveTiles)
            {
                tile.GetComponent<FallingTile>().OnStart();
            }

            for (var i = 0; i < _parallaxScrollingObjects.Count; i++)
            {
                var p = _parallaxScrollingObjects[i].transform;
                p.SetPositionAndRotation(_parallaxScrollingOriginalPositions[i], p.rotation);
            }
        }

        public void GameOver()
        {
            if (victoryMenu.gameObject.activeInHierarchy) return;
            gameOverMenu.gameObject.SetActive(true);
            _highScore.GameOver();
        }

        public void UpdateScore(int value)
        {
            _highScore.AddToScore(value);
            yourScoreText.text = "Your Score: " + _highScore.GetCurrentScore();
        }
    }
}