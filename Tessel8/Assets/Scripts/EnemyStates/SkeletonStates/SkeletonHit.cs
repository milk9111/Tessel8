using System.Collections;
using Hellmade.Sound;
using UnityEngine;
using UserInterface;

namespace EnemyStates.SkeletonStates
{
    public class SkeletonHit : BaseState
    {
        [Tooltip("The health of the enemy")]
        public int health = 30;

        public AudioClip enemyHitFx;

        public HealthBar healthBar;

        private bool _isDead;

        private bool _hasBeenHit;

        private int _currHealth;


        public override void Init()
        {
            _currHealth = health;
        }

        public override void DoAction()
        {
            if (_hasBeenHit) return;
            _animator.SetTrigger("Hit");
            _hasBeenHit = true;
        }
        
        public void DealDamage(int damage)
        {
            if (_isDead) return;
            _currHealth -= damage;

            healthBar.OnHit(damage / (float)health);
            
            EazySoundManager.PlaySound(enemyHitFx, false);
            _controller.ChangeState(States.Hit);

            if (_currHealth <= 0)
            {
                _isDead = true;
            }
        }

        public void FinishHit()
        {
            _hasBeenHit = false;
            _controller.ChangeState(_isDead ? States.Dead : States.Idle);
        }

        
    }
}