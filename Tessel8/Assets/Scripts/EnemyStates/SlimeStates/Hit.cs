using System.Collections;
using UnityEngine;
using UserInterface;

namespace EnemyStates.SlimeStates
{
    public class Hit : BaseState
    {
        [Tooltip("The health of the enemy")]
        public int health = 30;

        public HealthBar healthBar;

        private bool _isDead;

        private bool _hasBeenHit;

        public int _currHealth;


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

            PlaySoundFx();
            healthBar.OnHit(damage / (float)health);
            
            _controller.ChangeState(States.Hit);

            if (_currHealth <= 0)
            {
                _isDead = true;
            }
        }
        
        public int GetCurrentHealth()
        {
            return _currHealth;
        }

        public void FinishHit()
        {
            _hasBeenHit = false;
            _controller.ChangeState(_isDead ? States.Dead : States.Idle);
        }

        
    }
}