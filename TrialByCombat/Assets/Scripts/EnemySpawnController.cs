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

        public Grid grid;
        
        [Tooltip("The total number of enemies to spawn")]
        public int numberOfEnemies;

        [Tooltip("The time between enemy spawns in seconds")]
        public float timeBetweenEnemies;
        
        private HashSet<EnemyController> _enemies;
        private HashSet<Vector3> _validSpawnPositions;

        private bool _isCooldownFinished;

        private Vector2 _gridCellSize;
        
        void Awake()
        {
            _gridCellSize = grid.cellSize;
            
            _enemies = new HashSet<EnemyController>();
            GatherStartingEnemies();
            
            _validSpawnPositions = null;
            _isCooldownFinished = true;
        }

        void Update()
        {
            if (_validSpawnPositions == null)
            {
                _validSpawnPositions = new HashSet<Vector3>();
                GatherValidSpawnPositions();
            }
            
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