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

        private bool _isCooldownFinished;
        private bool _isPaused;
        
        void Awake()
        {            
            _enemies = new HashSet<EnemyController>();
            GatherStartingEnemies();
            ClearDeadEnemies();
            
            _validSpawnPositions = null;
            _isCooldownFinished = true;

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
                Debug.Log("Starting round " + (_roundIndex + 1) + ": " + _currRound.GetName());
                _currRound.Init(_validSpawnPositions, _enemies);
            }

            ClearDeadEnemies();

            if (_currRound.IsRoundComplete())
            {
                _roundIndex++;
                if (_roundIndex >= rounds.Length)
                {
                    Debug.Log("Finished all rounds!");
                    return;
                }
                _currRound = rounds[_roundIndex].GetSelectedRound();
                Debug.Log("Starting round " + (_roundIndex + 1) + ": " + _currRound.GetName());
                _currRound.Init(_validSpawnPositions, _enemies);
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
        }

        public void OnStart()
        {
            foreach (var enemy in _enemies)
            {
                enemy.MarkAsDead();
            }
            
            ClearDeadEnemies();

            _currRound = rounds[0].GetSelectedRound();
            _currRound.Init(_validSpawnPositions, _enemies);
            _roundIndex = 0;
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