using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnemyControllers;
using UnityEngine;

namespace Spawn.Domain.Round
{
    public class TimedWaveRound : BaseRound
    {
        public WaveConfiguration[] waveConfigurations;

        public float secondsBetweenWaves = 30;

        private int _waveIndex;

        private float _remainingSecondsOnTimer;

        private bool _isTimerOn;

        private Coroutine _lastCoroutine;

        private string _name;
                
        public override void Init(HashSet<Vector3> spawnPositions, HashSet<EnemyController> enemies)
        {
            _validSpawnPositions = spawnPositions;
            _enemies = enemies;
            _currWave = waveConfigurations[0];
            _waveIndex = 0;
            Debug.Log("Starting wave " + (_waveIndex + 1) + ": " + _currWave.GetName());

            _remainingSecondsOnTimer = 0;
            
            _isTimerOn = true;
            _lastCoroutine = StartCoroutine(WaveTimer());
        }
        
        public override void UpdateRound()
        {
            if (_isPaused) return;
            
            if (!_isTimerOn)
            {
                _waveIndex++;
                if (_waveIndex >= waveConfigurations.Length)
                {
                    _isComplete = !_enemies.Any();
                    return;
                }

                _currWave = waveConfigurations[_waveIndex];
                Debug.Log("Time's up! Starting wave " + (_waveIndex + 1) + ": " + _currWave.GetName());
            }
            
            if (!_isTimerOn && _waveIndex < waveConfigurations.Length)
            {
                _isTimerOn = true;
                _lastCoroutine = StartCoroutine(WaveTimer());
            }

            var enemy = _currWave.SpawnEnemy(RandomPositionFromValidPositions());
            if (enemy == null) return;
            
            _enemies.Add(enemy);
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
            return gameObject.name;
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