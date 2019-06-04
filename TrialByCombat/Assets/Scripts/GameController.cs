using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameController : MonoBehaviour
    {
        private EnemySpawnController _spawnController;
        
        void Awake()
        {
            _spawnController = GetComponent<EnemySpawnController>();
        }

        public IList<EnemyController> GetAllEnemiesInGame()
        {
            return _spawnController.GetAllEnemiesInGame();
        }
    }
}