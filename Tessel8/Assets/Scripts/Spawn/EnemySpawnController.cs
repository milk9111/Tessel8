using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnemyControllers;
using Spawn.Domain;
using Spawn.Domain.Round;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spawn
{
    public class EnemySpawnController : MonoBehaviour
    {
        public RoundHolder[] rounds;

        [Tooltip("The grid that contains all the platform tiles for the enemies to spawn on. Only works if useDesignatedSpawnPoints = false.")]
        public Grid grid;

        [Tooltip("The array of designated spawn points. Only works if useDesignatedSpawnPoints = true.")]
        public Transform[] spawnPoints;
        
        private HashSet<EnemyController> _enemies;
        private HashSet<Vector3> _validSpawnPositions;

        private IRound _currRound;
        private int _roundIndex;

        private SpawnUIController _spawnUiController;

        private bool _isCooldownFinished;
        private bool _isPaused;
        private bool _isFinished;
        
        void Awake()
        {
            _spawnUiController = GetComponent<SpawnUIController>();
            
            _enemies = new HashSet<EnemyController>();
            GatherStartingEnemies();
            ClearDeadEnemies();
            
            _validSpawnPositions = null;
            _isCooldownFinished = true;
            _isFinished = false;

            if (rounds.Length == 0)
            {
                _isPaused = true;
                return;
            }
            _currRound = rounds[0].GetSelectedRound();
            _roundIndex = 0;
        }

        void Update()
        {
            if (_isPaused) return;
            
            if (_validSpawnPositions == null)
            {
                _validSpawnPositions = new HashSet<Vector3>();
                GatherValidSpawnPositions();
                _spawnUiController.NewRound(RoundTypesHelper.GetTypeFromName(_currRound.GetName()), 
                    _currRound, _roundIndex + 1);
                _currRound.Init(_validSpawnPositions, _enemies, _spawnUiController);
            }

            ClearDeadEnemies();

            if (_currRound.IsRoundComplete())
            {
                _roundIndex++;
                if (_roundIndex >= rounds.Length)
                {
                    if (!_isFinished)
                    {
                        _isFinished = true;
                        _spawnUiController.NewVictory();
                    }
                    return;
                }
                _currRound = rounds[_roundIndex].GetSelectedRound();
                _spawnUiController.NewRound(RoundTypesHelper.GetTypeFromName(_currRound.GetName()), 
                    _currRound, _roundIndex + 1);
                _currRound.Init(_validSpawnPositions, _enemies, _spawnUiController);
            }
            
            _currRound.UpdateRound();
        }
        
        public IList<EnemyController> GetAllEnemiesInGame()
        {
            return _enemies.ToList();
        }

        public void OnPause()
        {
            _isPaused = true;
            
            _currRound.OnPause();
            
            foreach (var enemy in _enemies)
            {
                enemy.OnPause();
                enemy.enabled = false;
            }

            foreach (var bullet in GameObject.FindGameObjectsWithTag("Projectile"))
            {
                var proj = bullet.GetComponent<Projectile>();
                proj.OnPause();
                proj.enabled = false;
            }
        }

        public void OnPlay()
        {
            _isPaused = false;
            
            _currRound.OnPlay();
            
            foreach (var enemy in _enemies)
            {
                enemy.enabled = true;
                enemy.OnPlay();
            }
            
            foreach (var bullet in GameObject.FindGameObjectsWithTag("Projectile"))
            {
                var proj = bullet.GetComponent<Projectile>();
                proj.OnPlay();
                proj.enabled = true;
            }
        }

        public void OnStart()
        {
            foreach (var bullet in GameObject.FindGameObjectsWithTag("Projectile"))
            {
                Destroy(bullet);
            }
            
            foreach (var enemy in _enemies)
            {
                enemy.MarkAsDead();
            }

            foreach (var round in rounds)
            {
                round.GetSelectedRound().ResetRound();
            }
            
            ClearDeadEnemies();

            if (rounds.Length == 0) return;
            _currRound = rounds[0].GetSelectedRound();
            _roundIndex = 0;
            _validSpawnPositions = null;
            _isFinished = false;
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
            ValidDesignatedSpawnPointPositions();
        }

        private void ValidDesignatedSpawnPointPositions()
        {
            foreach (var spawnPoint in spawnPoints)
            {
                _validSpawnPositions.Add(spawnPoint.position);
            }
        }
    }
}