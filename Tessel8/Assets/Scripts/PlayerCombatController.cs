using System;
using System.Collections.Generic;
using EnemyControllers;
using EnemyStates;
using UnityEngine;
using Hellmade.Sound;
using UserInterface;

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

        public HealthBar healthBar;
        

        private PlayerTeleportController _teleportController;

        private PlayerPlatformerController _platformerController;
        
        private bool _isDead;

        private int _currHealth;

        void Awake()
        {
            _isDead = false;
            _teleportController = GetComponent<PlayerTeleportController>();
            _platformerController = GetComponent<PlayerPlatformerController>();
            _currHealth = health;
        }

        void Update()
        {            
            if (!animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).IsName("Player_SpinKick")
                && !_teleportController.IsTeleportRangeActivated() && (Input.GetKeyDown(KeyCode.Mouse0) 
                                                                       || Input.GetKeyDown(KeyCode.Q)))
            {
                animator.SetBool("Attacking", true);
            }
        }
        
        public void DealDamage(int damage)
        {
            if (_isDead) return;
            
            animator.SetBool("Attacking", false);
            animator.SetTrigger("Hit");
            _currHealth -= damage;
            
            healthBar.OnHit(damage / (float)health);
            
            EazySoundManager.PlaySound(playerHitFx, false);
            
            if (_currHealth <= 0)
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
        }

        public void FinishDead()
        {
            _isDead = true;
            _platformerController.DisableMovement();
            _teleportController.DisableTeleport();
            gameController.GameOver();
        }

        public void ResetHealth()
        {
            _isDead = false;
            _currHealth = health;
            healthBar.ResetHealthBar();
        }

        public void Pause()
        {
            _isDead = true;
        }

        public void Resume()
        {
            _isDead = false;
        }

        private bool IsWithinAttackRange(EnemyController enemy)
        {
            return Math.Abs(transform.position.x - enemy.transform.position.x) <= attackRange
                && Math.Abs(transform.position.y - enemy.transform.position.y) <= attackRange;
        }
    }
}