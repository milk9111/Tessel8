using EnemyStates.AntiTeleportStates;
using UnityEngine;

namespace StateMachineBehaviours
{
    public class AntiTeleportSlimeAttackBehaviour : StateMachineBehaviour
    {
        private AntiTeleportAttacking _attacking;

        private bool _hasAttacked;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            _hasAttacked = false;
            if (_attacking == null)
            {
                _attacking = animator.gameObject.GetComponentInParent<AntiTeleportAttacking>();
                if (_attacking == null)
                {
                    _attacking = animator.gameObject.GetComponent<AntiTeleportAttacking>();
                }
            }
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (!_hasAttacked && animatorStateInfo.normalizedTime >= 0.8f && animatorStateInfo.normalizedTime <= 1f)
            {
                _hasAttacked = true;
                _attacking.AttackPlayer();
            }
        }
    }
}