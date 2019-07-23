using EnemyStates.SkeletonStates;
using UnityEngine;

namespace EnemyControllers
{
    public class SkeletonControllerHelper : MonoBehaviour
    {
        private SkeletonAttacking _attacking;
        private SkeletonDead _dead;
        private SkeletonHit _hit;

        void Start()
        {
            _attacking = GetComponentInParent<SkeletonAttacking>();
            _dead = GetComponentInParent<SkeletonDead>();
            _hit = GetComponentInParent<SkeletonHit>();
        }

        public void AttackPlayer()
        {
            _attacking.AttackPlayer();
        }

        public void FinishDeath()
        {
            _dead.FinishDeath();
        }

        public void FinishHit()
        {
            _hit.FinishHit();
        }
    }
}