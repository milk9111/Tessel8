using UnityEngine;

namespace Spawn.Domain.Round
{
    public class PickupConfiguration : MonoBehaviour
    {
        public GameObject pickupPrefab;

        [Range(0.01f, 1)]
        public float spawnChance;

        //public int maxNumberOfSpawns;

        public string GetName()
        {
            return gameObject.name;
        }
    }
}