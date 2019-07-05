using System.Collections;
using DefaultNamespace;
using EnemyControllers;
using UnityEngine;

namespace EnemyStates.LaserSkeletonStates
{
    public class LaserSkeletonAttacking : BaseState
    {
        [Tooltip("The player's health component")]
        public PlayerCombatController playerCombat;

        [Tooltip("The laser wall prefab game object to instantiate on attack")]
        public GameObject laserWallPrefab;
        
        [Tooltip("The enemy's damage output")]
        public int damageOutput = 10;

        [Tooltip("The time in seconds between each attack")]
        public float secondsBetweenAttacks = 1f;

        private bool _isReadyToAttack;

        void Awake()
        {
            _isReadyToAttack = true;

            if (playerCombat == null)
            {
                playerCombat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombatController>();
            }
        }
                
        public override void DoAction()
        {
            if (!_isReadyToAttack)
            {
                _animator.SetBool("Attacking", false);
                _controller.ChangeState(States.Walking);
                return;
            }
            
            _animator.SetBool("Attacking", true);
        }

        public void AttackPlayer()
        {
            StartCoroutine(AttackCooldown());
            if (_controller.IsPlayerWithinStoppingDistance())
            {
                var direction = _controller.GetDirection();
                
                var laserWall = Instantiate(laserWallPrefab, 
                    new Vector3(direction < 0 ? transform.position.x - 1 : transform.position.x + 1,
                        transform.position.y), laserWallPrefab.transform.rotation);
                var wallController = laserWall.GetComponent<Projectile>();
                wallController.SetDirection(direction);
                wallController.SetDamage(damageOutput);
            }

            _animator.SetBool("Attacking", false);
            _controller.ChangeState(States.Walking);
        }

        IEnumerator AttackCooldown()
        {
            _isReadyToAttack = false;
            yield return new WaitForSeconds(secondsBetweenAttacks);
            _isReadyToAttack = true;
        }
    }
}