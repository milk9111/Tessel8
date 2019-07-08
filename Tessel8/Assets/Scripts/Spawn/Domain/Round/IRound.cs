using System.Collections.Generic;
using EnemyControllers;
using UnityEngine;

namespace Spawn.Domain.Round
{
    public interface IRound
    {
        bool IsRoundComplete();

        void UpdateRound();

        void Init(HashSet<Vector3> spawnPositions, HashSet<EnemyController> enemies, SpawnUIController spawnUiController);

        void SetSpawnPositions(HashSet<Vector3> spawnPositions);

        void OnPause();

        void OnPlay();

        string GetName();

        float GetRemainingTime();
    }
}