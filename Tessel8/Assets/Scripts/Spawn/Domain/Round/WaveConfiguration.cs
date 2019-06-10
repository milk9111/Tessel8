using System;
using System.Collections;
using System.Collections.Generic;
using EnemyControllers;
using UnityEngine;
using Random = System.Random;

namespace Spawn.Domain.Round
{
    public class WaveConfiguration : MonoBehaviour
    {
        public int numberOfEnemies;

        public float secondsBetweenEnemies;

        public EnemyConfiguration[] enemyTypes;

        private bool _isComplete;
        private bool _isCooldownFinished;

        private int _numberOfSpawnedEnemies;

        private IDictionary<ChanceRange, EnemyConfiguration> _ranges;

        void Awake()
        {
            _isCooldownFinished = true;
            
            _ranges = new Dictionary<ChanceRange, EnemyConfiguration>();

            var lastMax = 0f;
            foreach (var config in enemyTypes)
            {
                var range = new ChanceRange
                {
                    max = lastMax + config.spawnChance,
                    min = lastMax + 0.01f
                };
                lastMax = range.max;
                
                _ranges.Add(range, config);
            }
        }

        public bool IsWaveComplete()
        {
            return _isComplete;
        }

        public EnemyController SpawnEnemy(Vector3 pos)
        {
            if (_isComplete || !_isCooldownFinished) return null;

            var enemyConfig = GetEnemyConfiguration((float)Math.Round(UnityEngine.Random.Range(0.01f, 1f), 2));
            
            var newEnemy = Instantiate(enemyConfig.enemyPrefab, pos, enemyConfig.enemyPrefab.transform.rotation);

            _numberOfSpawnedEnemies++;
            if (_numberOfSpawnedEnemies >= numberOfEnemies)
            {
                _isComplete = true;
            }
            else
            {
                _isCooldownFinished = false;
                StartCoroutine(EnemySpawnCooldown());
            }

            return newEnemy.GetComponent<EnemyController>();
        }

        private EnemyConfiguration GetEnemyConfiguration(float perc)
        {
            foreach (var range in _ranges.Keys)
            {
                if (!range.IsWithinChanceRange(perc)) continue;

                return _ranges[range];
            }

            return null;
        }
        
        private IEnumerator EnemySpawnCooldown()
        {
            yield return new WaitForSeconds(secondsBetweenEnemies);
            _isCooldownFinished = true;
        }

        private class ChanceRange
        {
            public float min;
            public float max;

            public bool IsWithinChanceRange(float val)
            {
                return val >= min && val <= max;
            }
        }
    }
}