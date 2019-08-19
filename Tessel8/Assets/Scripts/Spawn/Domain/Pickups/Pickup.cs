namespace Spawn.Domain.Pickups
{
    public class Pickup : PhysicsObject
    {
        private bool _isDead;

        void Awake()
        {
            _isDead = false;
        }
        
        public void OnPause()
        {
            
        }

        public void OnPlay()
        {
            
        }
        
        public bool HasDied()
        {
            return _isDead;
        }

        public void MarkAsDead()
        {
            _isDead = true;
        }
    }
}