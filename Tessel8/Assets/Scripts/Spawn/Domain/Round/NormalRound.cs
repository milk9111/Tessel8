using System;
using System.Collections.Generic;
using System.Linq;
using EnemyControllers;
using UnityEngine;

namespace Spawn.Domain.Round
{
    public class NormalRound : BaseRound
    {
        public WaveConfiguration _waveConfiguration;
        
        private WaveConfiguration[] waveConfigurations;

        private int _waveIndex;
                
        public override void Init(HashSet<Vector3> spawnPositions, HashSet<EnemyController> enemies)
        {
            waveConfigurations = new[] { _waveConfiguration };
            
            _validSpawnPositions = spawnPositions;
            _enemies = enemies;
            _currWave = waveConfigurations[0];
            _waveIndex = 0;
            Debug.Log("Starting wave " + (_waveIndex + 1) + ": " + _currWave.GetName());
        }
        
        public override void UpdateRound()
        {
            if (_isPaused) return;
            
            if (_currWave.IsWaveComplete() && !_enemies.Any())
            {
                _waveIndex++;
                if (_waveIndex >= waveConfigurations.Length)
                {
                    _isComplete = true;
                    return;
                }

                _currWave = waveConfigurations[_waveIndex];
                Debug.Log("Starting wave " + (_waveIndex + 1) + ": " + _currWave.GetName());
            }

            var enemy = _currWave.SpawnEnemy(RandomPositionFromValidPositions());
            if (enemy == null) return;
            
            _enemies.Add(enemy);
        }
        
        public override string GetName()
        {
            return gameObject.name;
        }
        
        public override void OnPause()
        {
            base.OnPause();
        }

        public override void OnPlay()
        {
            base.OnPlay();
        }
    }
}