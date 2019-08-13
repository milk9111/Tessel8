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
        private bool _isPaused;
        private bool _overrideNumberOfEnemies;

        private float _overrideSecondsBetweenEnemies;

        private int _numberOfSpawnedEnemies;

        private IDictionary<ChanceRange, EnemyConfiguration> _ranges;

        private Coroutine _lastCoroutine;

        void Awake()
        {
            _overrideSecondsBetweenEnemies = 0;
            _overrideNumberOfEnemies = false;
            
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

        public void ResetWave()
        {
            _isComplete = false;
            _isPaused = false;
            _isCooldownFinished = true;
            _numberOfSpawnedEnemies = 0;
        }

        public EnemyController SpawnEnemy(Vector3 pos)
        {
            if (_isComplete || !_isCooldownFinished || _isPaused) return null;

            var enemyConfig = GetEnemyConfiguration((float)Math.Round(UnityEngine.Random.Range(0.01f, 1f), 2));
            
            var newEnemy = Instantiate(enemyConfig.enemyPrefab, pos, enemyConfig.enemyPrefab.transform.rotation);

            _numberOfSpawnedEnemies++;
            if (!_overrideNumberOfEnemies && _numberOfSpawnedEnemies >= numberOfEnemies)
            {
                _isComplete = true;
            }
            else
            {
                _isCooldownFinished = false;
                _lastCoroutine = StartCoroutine(EnemySpawnCooldown());
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

        public void OnPause()
        {
            if (_lastCoroutine != null)
            {
                StopCoroutine(_lastCoroutine);
            }

            _isPaused = true;
            _isCooldownFinished = true;
        }

        public void OnPlay()
        {
            _isPaused = false;
            _isCooldownFinished = false;
            _lastCoroutine = StartCoroutine(EnemySpawnCooldown());
        }

        public void OverrideSecondsBetweenEnemies(float overrideTime)
        {
            Debug.Log("Overriding seconds between enemies");
            _overrideSecondsBetweenEnemies = overrideTime;
        }

        public void OverrideNumberOfEnemies()
        {
            Debug.Log("Overriding number of enemies");
            _overrideNumberOfEnemies = true;
        }

        public string GetName()
        {
            return gameObject.name;
        }
        
        private IEnumerator EnemySpawnCooldown()
        {
            var timerLength = _overrideSecondsBetweenEnemies > 0
                ? _overrideSecondsBetweenEnemies
                : secondsBetweenEnemies;

            yield return new WaitForSeconds(timerLength);
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