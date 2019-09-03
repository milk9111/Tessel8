using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public class BackgroundMusicController : MonoBehaviour
    {
        public float secondsBetweenSongs = 5f;
        
        private AudioManager _audioManager;

        private Queue<Guid> _audioGuids;

        private bool _init;
        private bool _cooldown;
        private bool _isPaused;
        
        private Guid _currAudioGuid;
        private float _remainingSecondsOnTimer;
        private Coroutine _coroutine;
        
        void Awake()
        {
            _audioManager = FindObjectOfType<AudioManager>();
            OnStart();
        }

        public void OnStart()
        {
            if (!IsNullOrEmpty(_currAudioGuid) && _audioManager.IsPlaying(_currAudioGuid))
            {
                _audioManager.Stop(_currAudioGuid);
            }
            
            _audioGuids = new Queue<Guid>();
            _init = true;
            _currAudioGuid = Guid.Empty;
            _coroutine = null;
        }

        void Update()
        { 
            if (_init)
            {
                _init = false;
                foreach (var song in _audioManager.GetAllMusic())
                {
                    _audioGuids.Enqueue(_audioManager.PrepareMusic(song));
                }
            }

            if (_cooldown || !IsNullOrEmpty(_currAudioGuid) && _audioManager.IsPlaying(_currAudioGuid)) return;
            
            if (!IsNullOrEmpty(_currAudioGuid))
            {
                _audioGuids.Enqueue(_currAudioGuid);
            }
                
            _currAudioGuid = _audioGuids.Dequeue();
            _coroutine = StartCoroutine(SongCooldown());
        }
        
        public void ToggleMusic()
        {
            _isPaused = _audioManager.ToggleMusic();

            if (!_isPaused)
            {
                StopCoroutine(_coroutine);
            }
            else
            {
                _coroutine = StartCoroutine(SongCooldown());
            }
        } 
        
        private IEnumerator SongCooldown()
        {
            _cooldown = true;
            
            var timerLength = _remainingSecondsOnTimer > 0 ? _remainingSecondsOnTimer : secondsBetweenSongs;
            for (_remainingSecondsOnTimer = timerLength;
                _remainingSecondsOnTimer > 0;
                _remainingSecondsOnTimer -= Time.deltaTime)
            {
                yield return null;
            }

            _cooldown = false;
            _audioManager.Play(_currAudioGuid);
        }

        private bool IsNullOrEmpty(Guid guid)
        {
            return guid == null || guid == Guid.Empty;
        }
    }
}