using EnemyStates.LaserSkeletonStates;
using UnityEngine;

namespace StateMachineBehaviours
{
    public class LaserSlimeAttackBehaviour : StateMachineBehaviour
    {
        private LaserSkeletonAttacking _attacking;

        private bool _hasAttacked;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            _hasAttacked = false;
            if (_attacking == null)
            {
                _attacking = animator.gameObject.GetComponentInParent<LaserSkeletonAttacking>();
                if (_attacking == null)
                {
                    _attacking = animator.gameObject.GetComponent<LaserSkeletonAttacking>();
                }
            }
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (!_hasAttacked && animatorStateInfo.normalizedTime >= 0.6f && animatorStateInfo.normalizedTime <= 1f)
            {
                _hasAttacked = true;
                _attacking.AttackPlayer();
            }
        }
    }
}