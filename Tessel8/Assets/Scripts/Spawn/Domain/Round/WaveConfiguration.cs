using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnemyControllers;
using Spawn.Domain.Pickups;
using UnityEngine;
using Random = System.Random;

namespace Spawn.Domain.Round
{
    public class WaveConfiguration : MonoBehaviour
    {
        public int numberOfEnemies;

        public float secondsBetweenEnemies = 2f;

        public float secondsBetweenPickups = 30f;

        public EnemyConfiguration[] enemyTypes;

        public PickupConfiguration[] pickupTypes;

        private bool _isComplete;
        private bool _isEnemyCooldownFinished;
        private bool _isPickupCooldownFinished;
        private bool _isPaused;
        private bool _overrideNumberOfEnemies;

        private float _overrideSecondsBetweenEnemies;
        private float _remainingSecondsOnTimer;

        private int _numberOfSpawnedEnemies;
        private int _numberOfSpawnedPickups;

        private IDictionary<ChanceRange, EnemyConfiguration> _enemyRanges;
        private IDictionary<ChanceRange, PickupConfiguration> _pickupRanges;

        private Coroutine _lastEnemyCoroutine;
        private Coroutine _lastPickupCoroutine;

        void Awake()
        {
            _overrideSecondsBetweenEnemies = 0;
            _overrideNumberOfEnemies = false;
            
            _isEnemyCooldownFinished = true;
            _isPickupCooldownFinished = true;
            
            _enemyRanges = new Dictionary<ChanceRange, EnemyConfiguration>();
            _pickupRanges = new Dictionary<ChanceRange, PickupConfiguration>();

            var lastMax = 0f;
            foreach (var config in enemyTypes)
            {
                var range = new ChanceRange
                {
                    max = lastMax + config.spawnChance,
                    min = lastMax + 0.01f
                };
                lastMax = range.max;
                
                _enemyRanges.Add(range, config);
            }
            
            lastMax = 0f;
            foreach (var config in pickupTypes)
            {
                var range = new ChanceRange
                {
                    max = lastMax + config.spawnChance,
                    min = lastMax + 0.01f
                };
                lastMax = range.max;
                
                _pickupRanges.Add(range, config);
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
            _isEnemyCooldownFinished = true;
            _numberOfSpawnedEnemies = 0;
        }

        public EnemyController SpawnEnemy(Vector3 pos)
        {
            if (_isComplete || !_isEnemyCooldownFinished || _isPaused) return null;

            var enemyConfig = GetEnemyConfiguration((float)Math.Round(UnityEngine.Random.Range(0.01f, 1f), 2));
            
            var newEnemy = Instantiate(enemyConfig.enemyPrefab, pos, enemyConfig.enemyPrefab.transform.rotation);

            _numberOfSpawnedEnemies++;
            if (!_overrideNumberOfEnemies && _numberOfSpawnedEnemies >= numberOfEnemies)
            {
                _isComplete = true;
            }
            else
            {
                _isEnemyCooldownFinished = false;
                _lastEnemyCoroutine = StartCoroutine(EnemySpawnCooldown());
            }

            return newEnemy.GetComponent<EnemyController>();
        }

        public Pickup SpawnPickup(Vector3 pos)
        {
            if (_isComplete || !_isPickupCooldownFinished || _isPaused || !_pickupRanges.Any()) return null;

            var pickupConfig = GetPickupConfiguration((float)Math.Round(UnityEngine.Random.Range(0.01f, 1f), 2));
            if (pickupConfig == null)
            {
                return null;
            }
            
            var newPickup = Instantiate(pickupConfig.pickupPrefab, pos, pickupConfig.pickupPrefab.transform.rotation);

            _isPickupCooldownFinished = false;
            _lastPickupCoroutine = StartCoroutine(PickupSpawnCooldown());

            return newPickup.GetComponent<Pickup>();
        }

        private EnemyConfiguration GetEnemyConfiguration(float perc)
        {
            foreach (var range in _enemyRanges.Keys)
            {
                if (!range.IsWithinChanceRange(perc)) continue;

                return _enemyRanges[range];
            }

            return null;
        }
        
        private PickupConfiguration GetPickupConfiguration(float perc)
        {
            foreach (var range in _pickupRanges.Keys)
            {
                if (!range.IsWithinChanceRange(perc)) continue;

                return _pickupRanges[range];
            }

            return null;
        }

        public void OnPause()
        {
            if (_lastEnemyCoroutine != null)
            {
                StopCoroutine(_lastEnemyCoroutine);
            }

            if (_lastPickupCoroutine != null)
            {
                StopCoroutine(_lastPickupCoroutine);
            }

            _isPaused = true;
            _isEnemyCooldownFinished = true;
            _isPickupCooldownFinished = true;
        }

        public void OnPlay()
        {
            _isPaused = false;
            _isEnemyCooldownFinished = false;
            _lastEnemyCoroutine = StartCoroutine(EnemySpawnCooldown());
            _lastPickupCoroutine = StartCoroutine(PickupSpawnCooldown());
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
            var timerLength = _remainingSecondsOnTimer > 0 ? _remainingSecondsOnTimer : _overrideSecondsBetweenEnemies > 0
                ? _overrideSecondsBetweenEnemies
                : secondsBetweenEnemies;;
            for(_remainingSecondsOnTimer = timerLength; _remainingSecondsOnTimer > 0; _remainingSecondsOnTimer -= Time.deltaTime)
                yield return null;

            _isEnemyCooldownFinished = true;
        }
        
        private IEnumerator PickupSpawnCooldown()
        {
            var timerLength = _remainingSecondsOnTimer > 0 ? _remainingSecondsOnTimer : secondsBetweenPickups;
            for(_remainingSecondsOnTimer = timerLength; _remainingSecondsOnTimer > 0; _remainingSecondsOnTimer -= Time.deltaTime)
                yield return null;

            _isPickupCooldownFinished = true;
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