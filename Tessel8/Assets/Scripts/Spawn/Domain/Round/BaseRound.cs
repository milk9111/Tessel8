using System.Collections.Generic;
using System.Linq;
using EnemyControllers;
using UnityEngine;

namespace Spawn.Domain.Round
{
    public class BaseRound : MonoBehaviour, IRound
    {
        protected bool _isComplete;
        protected bool _isPaused;
        
        protected WaveConfiguration _currWave;
        
        protected HashSet<Vector3> _validSpawnPositions;

        protected HashSet<EnemyController> _enemies;

        protected SpawnUIController _spawnUiController;

        public void SetSpawnPositions(HashSet<Vector3> spawnPositions)
        {
            _validSpawnPositions = spawnPositions;
        }
        
        public bool IsRoundComplete()
        {
            return _isComplete;
        }

        public virtual void UpdateRound()
        {
        }

        public virtual void Init(HashSet<Vector3> spawnPositions, HashSet<EnemyController> enemies, 
            SpawnUIController spawnUiController)
        {
            _spawnUiController = spawnUiController;
        }
        
        protected Vector3 RandomPositionFromValidPositions()
        {
            var arr = _validSpawnPositions.ToArray();
            return arr[Random.Range(0, arr.Length)];
        }

        public virtual void OnPause()
        {
            _currWave.OnPause();
            _isPaused = true;
        }

        public virtual void OnPlay()
        {
            _currWave.OnPlay();
            _isPaused = false;
        }

        public virtual string GetName()
        {
            return "Base Round";
        }

        public virtual float GetRemainingTime()
        {
            return 0.0f;
        }
    }
}