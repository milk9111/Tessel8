using System;
using Audio;

namespace Spawn.Domain.Pickups
{
    public class Pickup : PhysicsObject
    {
        public string pickupFxName;
        
        private bool _isDead;
        private bool _init;
        
        private AudioManager _audioManager;
        private Guid _audioGuid;

        void Awake()
        {
            _isDead = false;
            _init = true;
            _audioManager = FindObjectOfType<AudioManager>();
        }

        void Update()
        {
            if (_init)
            {
                _init = false;
                _audioGuid = _audioManager.PrepareSound(pickupFxName);
            }
        }

        protected void PlaySoundFx()
        {
            if (_audioManager != null && _audioGuid != Guid.Empty && !_audioManager.IsPlaying(_audioGuid))
            {
                _audioManager.Play(_audioGuid);
            }
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