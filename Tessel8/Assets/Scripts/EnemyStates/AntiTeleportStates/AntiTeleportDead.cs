using EnemyControllers;
using UnityEngine;

namespace EnemyStates.AntiTeleportStates
{
    public class AntiTeleportDead : BaseState
    {
        public GameObject antiTeleportRadiusPrefab;
        public bool isImmortal;

        private bool _hasDied;
        
        public override void DoAction()
        {
            if (isImmortal || _hasDied) return;
            antiTeleportRadiusPrefab.SetActive(false);
            _hasDied = true;
            _animator.SetTrigger("Dead");
        }

        public void FinishDeath()
        {
            PlaySoundFx();
            antiTeleportRadiusPrefab.GetComponent<AntiTeleportRadius>().ReleasePlayer();
            antiTeleportRadiusPrefab.SetActive(false);
            _controller.MarkAsDead();
        }
    }
}