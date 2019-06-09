using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class EnemySpawnController : MonoBehaviour
    {
        [Tooltip("The enemy prefab to spawn")]
        public GameObject enemyPrefab;

        [Tooltip("The grid that contains all the platform tiles for the enemies to spawn on. Only works if useDesignatedSpawnPoints = false.")]
        public Grid grid;

        [Tooltip("The array of designated spawn points. Only works if useDesignatedSpawnPoints = true.")]
        public Transform[] spawnPoints;
        
        [Tooltip("The total number of enemies to spawn")]
        public int numberOfEnemies;

        [Tooltip("The time between enemy spawns in seconds")]
        public float timeBetweenEnemies;

        [Tooltip("Use designated spawn points for enemies or let them randomly spawn around the arena (broken).")] 
        public bool useDesignatedSpawnPoints;
        
        private HashSet<EnemyController> _enemies;
        private HashSet<Vector3> _validSpawnPositions;

        private bool _isCooldownFinished;
        private bool _isPaused;

        private Vector2 _gridCellSize;
        
        void Awake()
        {
            _gridCellSize = grid.cellSize;
            
            _enemies = new HashSet<EnemyController>();
            GatherStartingEnemies();
            ClearDeadEnemies();
            
            _validSpawnPositions = null;
            _isCooldownFinished = true;
        }

        void Update()
        {
            if (_isPaused) return;
            
            if (_validSpawnPositions == null)
            {
                _validSpawnPositions = new HashSet<Vector3>();
                GatherValidSpawnPositions();
            }

            ClearDeadEnemies();
            
            if (!_isCooldownFinished) return;
            _isCooldownFinished = false;
            StartCoroutine(EnemySpawnCooldown());
            SpawnEnemy();
        }
        
        private void SpawnEnemy()
        {
            var newEnemy = Instantiate(enemyPrefab, RandomPositionFromValidPositions(), enemyPrefab.transform.rotation);
            _enemies.Add(newEnemy.GetComponent<EnemyController>());
        }
        
        public IList<EnemyController> GetAllEnemiesInGame()
        {
            return _enemies.ToList();
        }

        public void OnPause()
        {
            _isPaused = true;
            
            foreach (var enemy in _enemies)
            {
                enemy.OnPause();
                enemy.enabled = false;
            }
        }

        public void OnPlay()
        {
            _isPaused = false;
            
            foreach (var enemy in _enemies)
            {
                enemy.enabled = true;
                enemy.OnPlay();
            }
        }

        public void OnStart()
        {
            foreach (var enemy in _enemies)
            {
                enemy.MarkAsDead();
            }
        }

        private void ClearDeadEnemies()
        {
            IList<EnemyController> deadEnemies = new List<EnemyController>();
            foreach (var enemy in _enemies)
            {
                if (!enemy.HasDied()) continue;

                deadEnemies.Add(enemy);
            }

            foreach (var enemy in deadEnemies)
            {
                _enemies.Remove(enemy);
                Destroy(enemy.gameObject);
            }
        }

        private void GatherStartingEnemies()
        {
            var result = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in result)
            {
                _enemies.Add(enemy.GetComponent<EnemyController>());
            }
        }

        private void GatherValidSpawnPositions()
        {
            if (useDesignatedSpawnPoints)
            {
                ValidDesignatedSpawnPointPositions();
            }
            else
            {
                ValidTilePositions();
            }
        }

        private void ValidDesignatedSpawnPointPositions()
        {
            foreach (var spawnPoint in spawnPoints)
            {
                _validSpawnPositions.Add(spawnPoint.position);
            }
        }

        private void ValidTilePositions()
        {
            var tiles = GameTiles.instance.tiles;
            var borderTile = GameTiles.instance.borderTiles;

            foreach (var pos in tiles.Keys)
            {
                var abovePos = new Vector3(pos.x + enemyPrefab.transform.localScale.x, 
                    pos.y + _gridCellSize.y + enemyPrefab.transform.localScale.y);
                
                var floorPos = new Vector3(Mathf.FloorToInt(abovePos.x), Mathf.FloorToInt(abovePos.y));
                var ceilPos = new Vector3(Mathf.CeilToInt(abovePos.x), Mathf.CeilToInt(abovePos.y));
                if (tiles.ContainsKey(floorPos) || borderTile.ContainsKey(floorPos) 
                                                || tiles.ContainsKey(ceilPos) || borderTile.ContainsKey(ceilPos)) continue;

                _validSpawnPositions.Add(abovePos);
            }
        }

        private Vector3 RandomPositionFromValidPositions()
        {
            var arr = _validSpawnPositions.ToArray();
            return arr[Random.Range(0, arr.Length)];
        }

        private IEnumerator EnemySpawnCooldown()
        {
            yield return new WaitForSeconds(timeBetweenEnemies);
            _isCooldownFinished = true;
        }
    }
}