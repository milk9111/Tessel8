using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameController : MonoBehaviour
    {
        private HashSet<EnemyController> _enemies;
        
        void Awake()
        {
            _enemies = new HashSet<EnemyController>();
            GatherStartingEnemies();
        }

        public IList<EnemyController> GetAllEnemiesInGame()
        {
            return _enemies.ToList();
        }

        private void GatherStartingEnemies()
        {
            var result = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in result)
            {
                _enemies.Add(enemy.GetComponent<EnemyController>());
            }
        }
    }
}