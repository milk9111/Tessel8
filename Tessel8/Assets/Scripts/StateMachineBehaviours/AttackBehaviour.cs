using EnemyStates.SlimeStates;
using UnityEngine;

namespace StateMachineBehaviours
{
    public class AttackBehaviour : StateMachineBehaviour
    {
        private Attacking _attacking;

        private bool _hasAttacked;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            _hasAttacked = false;
            if (_attacking == null)
            {
                _attacking = animator.gameObject.GetComponentInParent<Attacking>();
                if (_attacking == null)
                {
                    _attacking = animator.gameObject.GetComponent<Attacking>();
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