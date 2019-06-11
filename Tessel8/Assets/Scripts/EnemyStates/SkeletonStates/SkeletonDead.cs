
using UnityEngine;

namespace EnemyStates.SkeletonStates
{
    public class SkeletonDead : BaseState
    {
        public bool isImmortal;
        
        public override void DoAction()
        {
            if (isImmortal) return;
            _animator.SetTrigger("Dead");
        }

        public void FinishDeath()
        {
            _controller.MarkAsDead();
        }
    }
}