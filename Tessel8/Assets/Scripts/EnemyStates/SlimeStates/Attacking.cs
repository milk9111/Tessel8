using System.Collections;
using DefaultNamespace;
using EnemyStates;
using UnityEngine;

namespace EnemyStates.SlimeStates
{
    public class Attacking : BaseState
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
                return;
            }
            
            _animator.SetBool("Attacking", true);
        }

        public void AttackPlayer()
        {
            PlaySoundFx();
            var isFacingLeft = _controller.GetDirection() < 0;
            var playerPos = playerCombat.gameObject.transform.position;

            var playerIsOnFacingDirection = isFacingLeft && playerPos.x <= transform.position.x
                                            || !isFacingLeft && playerPos.x >= transform.position.x;
            
            StartCoroutine(AttackCooldown());
            if (playerIsOnFacingDirection && _controller.IsPlayerWithinStoppingDistance())
            {
                playerCombat.DealDamage(damageOutput);
            }

            _animator.SetBool("Attacking", false);
            _controller.ChangeState(States.Walking);
        }

        IEnumerator AttackCooldown()
        {
            _isReadyToAttack = false;
            _controller.SetCanAttack(_isReadyToAttack);
            yield return new WaitForSeconds(secondsBetweenAttacks);
            _isReadyToAttack = true;
            _controller.SetCanAttack(_isReadyToAttack);
        }
    }
}