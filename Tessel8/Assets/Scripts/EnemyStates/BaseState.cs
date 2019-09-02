using System;
using System.Collections;
using Audio;
using EnemyControllers;
using UnityEngine;

namespace EnemyStates
{
    public class BaseState : MonoBehaviour, IState
    {
        [Tooltip("The associated sound effect name for this state")]
        public string soundFxName;
        
        [Tooltip("The seconds between each play of the sound effect")]
        public float secondsBetweenFx;
    
        protected EnemyController _controller;
        protected Animator _animator;
        protected bool _isPaused;

        private Coroutine _soundFxCoroutine;
        private float _remainingSecondsOnFxTimer;
        private bool _canPlayFx = true;
        private AudioManager _audioManager;
        private Guid _audioGuid;

        public virtual void Init() { }
        
        public virtual void DoAction() { }

        public virtual void OnPause()
        {
            if (_soundFxCoroutine != null)
            {
                StopCoroutine(_soundFxCoroutine);
            }

            if (_audioManager != null)
            {
                _audioManager.Pause(_audioGuid);
            }

            _isPaused = true;
        }

        public virtual void OnPlay()
        {
            if (_soundFxCoroutine != null)
            {
                _soundFxCoroutine = StartCoroutine(SoundFxCooldown());
            }

            if (_audioManager != null)
            {
                _audioManager.Resume(_audioGuid);
            }

            _isPaused = false;
        }

        protected void PlaySoundFx()
        {            
            if (_canPlayFx && _audioManager != null && !_audioManager.IsPlaying(_audioGuid))
            {
                _audioManager.Play(_audioGuid);
                _soundFxCoroutine = StartCoroutine(SoundFxCooldown());
            }
        }

        protected bool IsPaused()
        {
            return _isPaused;
        }

        public void SetupFields(EnemyController controller, Animator animator)
        {
            _controller = controller;

            if (animator == null)
            {
                animator = new Animator();
            }

            _animator = animator;

            if (!string.IsNullOrEmpty(soundFxName))
            {
                _audioManager = FindObjectOfType<AudioManager>();
                _audioGuid = _audioManager.PrepareSound(soundFxName);
            }
        }

        public void Delete()
        {
            if (_audioManager == null) return;
            _audioManager.Destroy(_audioGuid);
        }

        private IEnumerator SoundFxCooldown()
        {
            _canPlayFx = false;
            var timerLength = _remainingSecondsOnFxTimer > 0 ? _remainingSecondsOnFxTimer : secondsBetweenFx;
            for(_remainingSecondsOnFxTimer = timerLength; _remainingSecondsOnFxTimer > 0; _remainingSecondsOnFxTimer -= Time.deltaTime)
                yield return null;
            _canPlayFx = true;
        }
    }
}