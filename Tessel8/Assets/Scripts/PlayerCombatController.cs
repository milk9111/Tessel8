using System;
using Audio;
using EnemyControllers;
using UnityEngine;
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

        public string attackFxName;
        public string hitFxName;
        public string healFxName;

        public HealthBar healthBar;

        [Tooltip("The name of the attack 'state', not the name of the attack 'animation'")]
        public string attackAnimatorStateName;

        private PlayerTeleportController _teleportController;

        private PlayerPlatformerController _platformerController;
        
        private bool _isDead;

        private bool _isAttacking;

        private int _currHealth;

        private AudioManager _audioManager;
        private Guid _hitFxAudioGuid;
        private Guid _attackFxAudioGuid;
        private Guid _healFxAudioGuid;

        void Awake()
        {
            _isDead = false;
            _teleportController = GetComponent<PlayerTeleportController>();
            _platformerController = GetComponent<PlayerPlatformerController>();
            _currHealth = health;
            if (string.IsNullOrEmpty(attackAnimatorStateName))
            {
                attackAnimatorStateName = "Player_SpinKick";
            }

            _audioManager = FindObjectOfType<AudioManager>();
        }

        void Start()
        {
            _hitFxAudioGuid = _audioManager.PrepareSound(hitFxName);
            _attackFxAudioGuid = _audioManager.PrepareSound(attackFxName);
            _healFxAudioGuid = _audioManager.PrepareSound(healFxName);
        }

        void Update()
        {            
            if (!animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer"))
                    .IsName(attackAnimatorStateName)
                && !_teleportController.IsTeleportRangeActivated() && (Input.GetKeyDown(KeyCode.Mouse0) 
                                                                       || Input.GetKeyDown(KeyCode.Q)))
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
            _currHealth -= damage;
            
            healthBar.OnHit(damage / (float)health);
            
            _audioManager.Play(_hitFxAudioGuid);
            
            if (_currHealth <= 0)
            {
                animator.SetTrigger("Dead");
            }
        }

        public void Heal(int heal)
        {
            //_audioManager.Play(_healFxAudioGuid);
            
            if (_isDead) return;
            
            var fillAmount = heal;
            var newHealth = _currHealth + heal;
            if (newHealth >= health)
            {
                fillAmount = health - _currHealth;
                newHealth = health;
            }    
            
            _currHealth = newHealth;
            
            healthBar.OnHit(fillAmount / (float)health * -1);
        }

        public void AttackEnemy()
        {
            _audioManager.Play(_attackFxAudioGuid);
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
            _isAttacking = false;
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