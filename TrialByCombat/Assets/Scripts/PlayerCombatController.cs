using System;
using System.Collections.Generic;
using EnemyStates;
using UnityEngine;
using Hellmade.Sound;

namespace DefaultNamespace
{
    public class PlayerCombatController : MonoBehaviour
    {
        [Tooltip("The amount of health the player has")]
        public int health = 100;

        [Tooltip("The amount of damage the player deals")]
        public int damageOutput = 20;

        [Tooltip("The attack range of the player")]
        public float attackRange;

        public GameController gameController;
        
        public Animator animator;

        public AudioClip playerHitFx;
        

        private PlayerTeleportController _teleportController;

        private PlayerPlatformerController _platformerController;
        
        private bool _isAttacking;

        private bool _isDead;

        void Awake()
        {
            _isDead = false;
            _isAttacking = false;
            _teleportController = GetComponent<PlayerTeleportController>();
            _platformerController = GetComponent<PlayerPlatformerController>();
        }

        void Update()
        {
            if (!animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).IsName("Player_SpinKick") && !_isAttacking 
                && !_teleportController.IsTeleportRangeActivated() && Input.GetKeyDown(KeyCode.Mouse0))
            {
                _isAttacking = true;
                animator.SetBool("Attacking", true);
            }
        }
        
        public void DealDamage(int damage)
        {
            if (_isDead) return;
            
            animator.SetBool("Attacking", false);
            animator.SetTrigger("Hit");
            health -= damage;
            EazySoundManager.PlaySound(playerHitFx, false);
            
            if (health <= 0)
            {
                animator.SetTrigger("Dead");
            }
        }

        public void AttackEnemy()
        {
            foreach (var enemy in gameController.GetAllEnemiesInGame())
            {
                if (enemy != null && IsWithinAttackRange(enemy))
                {
                    enemy.DealDamage(damageOutput);
                }
            }
        }

        public void FinishAttack()
        {
            animator.SetBool("Attacking", false);
            _isAttacking = false;
        }

        public void FinishDead()
        {
            _isDead = true;
            _platformerController.DisableMovement();
            _teleportController.DisableTeleport();
        }

        private bool IsWithinAttackRange(EnemyController enemy)
        {
            return Math.Abs(transform.position.x - enemy.transform.position.x) <= attackRange
                && Math.Abs(transform.position.y - enemy.transform.position.y) <= attackRange;
        }
    }
}