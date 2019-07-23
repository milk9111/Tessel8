using DefaultNamespace;
using UnityEngine;

namespace StateMachineBehaviours
{
    public class PlayerAttackBehaviour : StateMachineBehaviour
    {
        private PlayerCombatController _combatController;

        private bool _hasAttackedEnemy;
        private bool _hasFinishedAttack;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            _hasAttackedEnemy = false;
            _hasFinishedAttack = false;
            if (_combatController == null)
            {
                _combatController = animator.gameObject.GetComponentInParent<PlayerCombatController>();
            }
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (!_hasAttackedEnemy 
                && animatorStateInfo.normalizedTime >= 0.5f 
                && animatorStateInfo.normalizedTime <= 0.6f)
            {
                _hasAttackedEnemy = true;
                _combatController.AttackEnemy();
            } else if (!_hasFinishedAttack 
                       && animatorStateInfo.normalizedTime >= 0.8f
                       && animatorStateInfo.normalizedTime <= 1f)
            {
                _hasFinishedAttack = true;
                _combatController.FinishAttack();
            }
        }
    }
}