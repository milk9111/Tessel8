
using UnityEngine;

namespace EnemyStates.SkeletonStates
{
    public class SkeletonDead : BaseState
    {
        public bool isImmortal;

        private bool _hasDied;
        
        public override void DoAction()
        {
            if (isImmortal || _hasDied) return;
            _hasDied = true;
            _animator.SetTrigger("Dead");
        }

        public void FinishDeath()
        {
            _controller.MarkAsDead();
        }
    }
}