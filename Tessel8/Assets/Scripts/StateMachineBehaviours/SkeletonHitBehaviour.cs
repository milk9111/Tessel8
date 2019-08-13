using EnemyStates.SkeletonStates;
using UnityEngine;

namespace StateMachineBehaviours
{
    public class SkeletonHitBehaviour : StateMachineBehaviour
    {
        public float hitEventTimeStart = 0.8f;

        public float hitEventTimeEnd = 1f;
    
        private SkeletonHit _hit;

        private bool _hasBeenHit;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            _hasBeenHit = false;
            if (_hit == null)
            {
                _hit = animator.gameObject.GetComponentInParent<SkeletonHit>();
                if (_hit == null)
                {
                    _hit = animator.gameObject.GetComponent<SkeletonHit>();
                }
            }
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (!_hasBeenHit && animatorStateInfo.normalizedTime >= hitEventTimeStart && animatorStateInfo.normalizedTime <= hitEventTimeEnd)
            {
                _hasBeenHit = true;
                _hit.FinishHit();
            }
        }
    }
}