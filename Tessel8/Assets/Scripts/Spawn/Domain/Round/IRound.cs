using System.Collections.Generic;
using EnemyControllers;
using Spawn.Domain.Pickups;
using UnityEngine;

namespace Spawn.Domain.Round
{
    public interface IRound
    {
        bool IsRoundComplete();

        void ResetRound();

        void UpdateRound();

        void Init(HashSet<Vector3> spawnPositions, HashSet<EnemyController> enemies, 
            HashSet<Pickup> pickups, SpawnUIController spawnUiController);

        void SetSpawnPositions(HashSet<Vector3> spawnPositions);

        void OnPause();

        void OnPlay();

        string GetName();

        float GetRemainingTime();
    }
}