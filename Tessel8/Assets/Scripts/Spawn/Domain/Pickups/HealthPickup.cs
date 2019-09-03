using System;
using Audio;
using DefaultNamespace;
using UnityEngine;

namespace Spawn.Domain.Pickups
{
    public class HealthPickup : Pickup
    {
        public int healAmount = 10;

        protected override void ChildUpdate()
        {
            
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (HasDied() || !string.Equals(other.gameObject.tag, "Player"))
            {
                return;
            }
            
            PlaySoundFx();
            
            other.gameObject.GetComponent<PlayerCombatController>().Heal(healAmount);
            MarkAsDead();
        }
    }
}