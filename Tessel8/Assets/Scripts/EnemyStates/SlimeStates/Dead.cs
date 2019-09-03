
using UnityEngine;

namespace EnemyStates.SlimeStates
{
    public class Dead : BaseState
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
            PlaySoundFx();
            Debug.Log("Marking enemy for death: " + gameObject.name);
            _controller.MarkAsDead();
        }
    }
}