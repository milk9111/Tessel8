using EnemyStates.SlimeStates;
using UnityEngine;

namespace StateMachineBehaviours
{
    public class HitBehaviour : StateMachineBehaviour
    {
        public float hitEventTimeStart = 0.8f;

        public float hitEventTimeEnd = 1f;
    
        private Hit _hit;

        private bool _hasBeenHit;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            _hasBeenHit = false;
            if (_hit == null)
            {
                _hit = animator.gameObject.GetComponentInParent<Hit>();
                if (_hit == null)
                {
                    _hit = animator.gameObject.GetComponent<Hit>();
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