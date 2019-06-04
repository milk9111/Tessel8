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

        void Awake()
        {
            _teleportController = GetComponent<PlayerTeleportController>();
        }

        void Update()
        {
            if (!_teleportController.IsTeleportRangeActivated() && Input.GetKeyDown(KeyCode.Mouse0))
            {
                animator.SetBool("Attacking", true);
            }
        }
        
        public void DealDamage(int damage)
        {
            health -= damage;
            EazySoundManager.PlaySound(playerHitFx, false);
            
            if (health <= 0)
            {
                Debug.Log("Player is dead");
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
        }

        private bool IsWithinAttackRange(EnemyController enemy)
        {
            return Math.Abs(transform.position.x - enemy.transform.position.x) <= attackRange;
        }
    }
}