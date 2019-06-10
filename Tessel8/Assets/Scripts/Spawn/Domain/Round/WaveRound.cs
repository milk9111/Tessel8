
using System.Collections.Generic;
using System.Linq;
using EnemyControllers;
using UnityEngine;

namespace Spawn.Domain.Round
{
    public class WaveRound : BaseRound
    {
        public WaveConfiguration[] waveConfigurations;

        private int _waveIndex;
                
        public override void Init(HashSet<Vector3> spawnPositions, HashSet<EnemyController> enemies)
        {
            _validSpawnPositions = spawnPositions;
            _enemies = enemies;
            _currWave = waveConfigurations[0];
            _waveIndex = 0;
        }
        
        public override void UpdateRound()
        {
            if (_currWave.IsWaveComplete() && !_enemies.Any())
            {
                _waveIndex++;
                if (_waveIndex >= waveConfigurations.Length)
                {
                    _isComplete = true;
                    return;
                }

                _currWave = waveConfigurations[_waveIndex];
                Debug.Log("Starting wave " + (_waveIndex + 1));
            }

            var enemy = _currWave.SpawnEnemy(RandomPositionFromValidPositions());
            if (enemy == null) return;
            
            _enemies.Add(enemy);
        }
    }
}