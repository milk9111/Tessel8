using EnemyStates.SkeletonStates;
using UnityEngine;

namespace StateMachineBehaviours
{
    public class SkeletonDeadBehaviour : StateMachineBehaviour
    {
        private SkeletonDead _dead;

        private bool _hasDied;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            _hasDied = false;
            if (_dead == null)
            {
                _dead = animator.gameObject.GetComponentInParent<SkeletonDead>();
                if (_dead == null)
                {
                    _dead = animator.gameObject.GetComponent<SkeletonDead>();
                }
            }
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (!_hasDied && animatorStateInfo.normalizedTime >= 0.6f && animatorStateInfo.normalizedTime <= 1f)
            {
                _hasDied = true;
                _dead.FinishDeath();
            }
        }
    }
}