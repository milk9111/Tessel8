using System.Collections;
using DefaultNamespace;
using UnityEngine;

namespace EnemyStates
{
    public class EnemyAttacking : BaseState
    {
        [Tooltip("The player's health component")]
        public PlayerCombatController playerCombat;
        
        [Tooltip("The enemy's damage output")]
        public int damageOutput = 10;

        [Tooltip("The time in seconds between each attack")]
        public float secondsBetweenAttacks = 1f;

        private bool _isReadyToAttack;

        void Awake()
        {
            _isReadyToAttack = true;
        }
                
        public override void DoAction()
        {
            if (!_isReadyToAttack)
            {
                _animator.SetBool("Attacking", false);
                return;
            }
            
            //_animator.SetBool("Walking", false);
            _animator.SetBool("Attacking", true);
        }

        public void AttackPlayer()
        {
            StartCoroutine(AttackCooldown());
            if (_controller.IsPlayerWithinStoppingDistance())
            {
                playerCombat.DealDamage(damageOutput);
            }

            //_animator.SetBool("Walking", true);
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