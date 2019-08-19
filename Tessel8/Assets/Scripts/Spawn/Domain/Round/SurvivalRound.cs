using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnemyControllers;
using Spawn.Domain.Pickups;
using UnityEngine;

namespace Spawn.Domain.Round
{
    public class SurvivalRound : BaseRound
    {
        public float secondsBetweenWaves = 30;

        private int _waveIndex;

        private float _remainingSecondsOnTimer;

        private bool _isTimerOn;

        private Coroutine _lastCoroutine;
                
        public override void Init(HashSet<Vector3> spawnPositions, HashSet<EnemyController> enemies, 
            HashSet<Pickup> pickups, SpawnUIController spawnUiController)
        {
            base.Init(spawnPositions, enemies, pickups, spawnUiController);
            _validSpawnPositions = spawnPositions;
            _enemies = enemies;
            _currWave = waveConfigurations[0];
            _currWave.OverrideNumberOfEnemies();
            _waveIndex = 0;
            _spawnUiController.NewWave(_waveIndex + 1);
            _remainingSecondsOnTimer = 0;
            
            _isTimerOn = true;
            _lastCoroutine = StartCoroutine(WaveTimer());
        }
        
        public override void ResetRound()
        {
            base.ResetRound();
            foreach (var wave in waveConfigurations)
            {
                wave.ResetWave();
            }
        }
        
        public override void UpdateRound()
        {
            if (_isPaused) return;
            
            if (!_isTimerOn)
            {
                MarkAllEnemiesAsDead();
                _waveIndex++;
                if (_waveIndex >= waveConfigurations.Length)
                {
                    _isComplete = true;
                    return;
                }
                
                _currWave = waveConfigurations[_waveIndex];
                _spawnUiController.NewWave(_waveIndex + 1);
                _currWave.OverrideNumberOfEnemies();
            }
            
            if (!_isTimerOn && _waveIndex < waveConfigurations.Length)
            {
                _isTimerOn = true;
                _lastCoroutine = StartCoroutine(WaveTimer());
            }

            var enemy = _currWave.SpawnEnemy(RandomPositionFromValidPositions());
            if (enemy != null)
            {
                _enemies.Add(enemy);
            }

            var pickup = _currWave.SpawnPickup(RandomPositionFromValidPositions());
            if (pickup != null)
            {
                _pickups.Add(pickup);
            }
        }

        private void MarkAllEnemiesAsDead()
        {
            foreach (var enemy in _enemies)
            {
                enemy.MarkAsDead();
            }
        }

        public override void OnPause()
        {
            base.OnPause();
            _isTimerOn = false;
            StopCoroutine(_lastCoroutine);
        }

        public override void OnPlay()
        {
            base.OnPlay();
            _isTimerOn = true;
            _lastCoroutine = StartCoroutine(WaveTimer());
        }
        
        public override string GetName()
        {
            return GetType().Name;
        }
        
        public override float GetRemainingTime()
        {
            return _remainingSecondsOnTimer;
        }

        private IEnumerator WaveTimer()
        {
            var timerLength = _remainingSecondsOnTimer > 0 ? _remainingSecondsOnTimer : secondsBetweenWaves;
            for(_remainingSecondsOnTimer = timerLength; _remainingSecondsOnTimer > 0; _remainingSecondsOnTimer -= Time.deltaTime)
                yield return null;
            _isTimerOn = false;
        }
    }
}