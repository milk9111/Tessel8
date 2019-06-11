using System;
using UnityEngine;

namespace Spawn.Domain.Round
{
    public class EnemyConfiguration : MonoBehaviour
    {
        public GameObject enemyPrefab;

        [Range(0.01f, 1)]
        public float spawnChance;

        public int maxNumberOfSpawns;

        public string GetName()
        {
            return gameObject.name;
        }
    }
}