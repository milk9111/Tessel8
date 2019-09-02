using EnemyStates.SlimeStates;
using UnityEngine;

namespace EnemyControllers
{
    public class SlimeControllerHelper : MonoBehaviour
    {
        private Attacking _attacking;
        private Dead _dead;
        private Hit _hit;

        void Start()
        {
            _attacking = GetComponentInParent<Attacking>();
            _dead = GetComponentInParent<Dead>();
            _hit = GetComponentInParent<Hit>();
        }

        public void AttackPlayer()
        {
            _attacking.AttackPlayer();
        }

        public void FinishDeath()
        {
            _dead.FinishDeath();
        }

        public void FinishHit()
        {
            _hit.FinishHit();
        }
    }
}