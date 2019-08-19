using System;
using System.Collections.Generic;
using System.Linq;
using EnemyControllers;
using Spawn.Domain.Pickups;
using UnityEngine;

namespace Spawn.Domain.Round
{
    public class NormalRound : BaseRound
    {
        public WaveConfiguration _waveConfiguration;
        
        private int _waveIndex;
                
        public override void Init(HashSet<Vector3> spawnPositions, HashSet<EnemyController> enemies, 
            HashSet<Pickup> pickups, SpawnUIController spawnUiController)
        {
            base.Init(spawnPositions, enemies, pickups, spawnUiController);
            waveConfigurations = new[] { _waveConfiguration };
            
            _validSpawnPositions = spawnPositions;
            _enemies = enemies;
            _pickups = pickups;
            _currWave = waveConfigurations[0];
            _waveIndex = 0;
            _spawnUiController.NewWave(_waveIndex + 1);
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
            
            if (_currWave.IsWaveComplete() && !_enemies.Any())
            {
                _waveIndex++;
                if (_waveIndex >= waveConfigurations.Length)
                {
                    _isComplete = true;
                    return;
                }

                _currWave = waveConfigurations[_waveIndex];
                _spawnUiController.NewWave(_waveIndex + 1);
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
        
        public override string GetName()
        {
            return GetType().Name;
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