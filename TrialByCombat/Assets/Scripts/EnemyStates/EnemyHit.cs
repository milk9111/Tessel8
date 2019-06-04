using Hellmade.Sound;
using UnityEngine;

namespace EnemyStates
{
    public class EnemyHit : BaseState
    {
        [Tooltip("The health of the enemy")]
        public int health = 30;

        public AudioClip enemyHitFx;

        private bool _isDead;

        private bool _hasBeenHit;
        
        public override void DoAction()
        {
            if (_hasBeenHit) return;
            _animator.SetTrigger("Hit");
            _hasBeenHit = true;
        }
        
        public void DealDamage(int damage)
        {
            health -= damage;
            EazySoundManager.PlaySound(enemyHitFx, false);
            _controller.ChangeState(States.Hit);

            if (health <= 0)
            {
                Debug.Log("Enemy is dead");
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