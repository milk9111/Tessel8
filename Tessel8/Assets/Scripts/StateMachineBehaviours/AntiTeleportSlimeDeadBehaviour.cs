using EnemyStates.AntiTeleportStates;
using UnityEngine;

namespace StateMachineBehaviours
{
    public class AntiTeleportSlimeDeadBehaviour : StateMachineBehaviour
    {
        public float deadEventTimeStart = 0.6f;

        public float deadEventTimeEnd = 1f;
        
        private AntiTeleportDead _dead;

        private bool _hasDied;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            _hasDied = false;
            if (_dead == null)
            {
                _dead = animator.gameObject.GetComponentInParent<AntiTeleportDead>();
                if (_dead == null)
                {
                    _dead = animator.gameObject.GetComponent<AntiTeleportDead>();
                }
            }
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (!_hasDied && animatorStateInfo.normalizedTime >= deadEventTimeStart && animatorStateInfo.normalizedTime <= deadEventTimeEnd)
            {
                _hasDied = true;
                _dead.FinishDeath();
            }
        }
    }
}