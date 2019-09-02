using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;
using UnityEngine;
using UnityScript.Lang;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public Sound[] sounds;

        public static AudioManager instance;

        private IDictionary<string, Sound> _sounds;

        private IDictionary<Guid, GameObject> _currentMusicSources;
        private IDictionary<Guid, GameObject> _currentSoundFxSources;

        private bool _musicEnabled;
        private bool _soundFxEnabled;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);

            _musicEnabled = true;
            _soundFxEnabled = true;
            
            _currentMusicSources = new Dictionary<Guid, GameObject>();
            _currentSoundFxSources = new Dictionary<Guid, GameObject>();
            
            _sounds = new Dictionary<string, Sound>();
            foreach (var sound in sounds)
            {
                if (_sounds.ContainsKey(sound.name))
                {
                    Debug.LogError("Sound '" + sound.name +"' already exists");
                    return;
                }
                
                _sounds.Add(sound.name, sound);
            }
        }

        public Guid PrepareSound(string soundName)
        {
            Sound sound;
            if (!_sounds.TryGetValue(soundName, out sound))
            {
                Debug.LogWarning("Sound '" + soundName + "' doesn't exist");
                return Guid.Empty;
            }

            var source = BuildAudioSource(sound);
            var guid = Guid.NewGuid();
            source.name = soundName + "_" + guid;
            _currentSoundFxSources.Add(guid, source);
            source.transform.parent = gameObject.transform;

            return guid;
        }
        
        public Guid PrepareMusic(string soundName)
        {
            Sound sound;
            if (!_sounds.TryGetValue(soundName, out sound))
            {
                Debug.LogWarning("Sound '" + soundName + "' doesn't exist");
                return Guid.Empty;
            }

            var source = BuildAudioSource(sound);
            var guid = Guid.NewGuid();
            source.name = soundName + "_" + guid;
            _currentMusicSources.Add(guid, source);
            source.transform.parent = gameObject.transform;

            return guid;
        }

        public void Play(Guid guid)
        {
            var source = GetAudioSource(guid);

            if (source == null)
            {
                
                return;
            }
            
            source.Play();
        }

        public bool IsPlaying(Guid guid)
        {
            var source = GetAudioSource(guid);
            return source != null && source.isPlaying;
        }

        public void Pause(Guid guid)
        {
            var source = GetAudioSource(guid);

            if (source == null) return;
            
            source.Pause();
        }

        public void Destroy(Guid guid)
        {
            GameObject source;
            if (!_currentSoundFxSources.TryGetValue(guid, out source))
            {
                return;
            }
            
            Destroy(source);
            _currentSoundFxSources.Remove(guid);
        }

        public void Resume(Guid guid)
        {
            var source = GetAudioSource(guid);

            if (source == null) return;
            
            source.UnPause();
        }

        public void Stop(Guid guid)
        {
            var source = GetAudioSource(guid);

            if (source == null) return;
            
            source.Stop();
        }

        public void ToggleSoundFx()
        {
            _soundFxEnabled = !_soundFxEnabled;
            
            foreach (var source in _currentSoundFxSources.Values)
            {
                if (!_soundFxEnabled)
                {
                    source.GetComponent<AudioSource>().Stop();
                }
            }
        }
        
        public void ToggleMusic()
        {
            _musicEnabled = !_musicEnabled;
            
            foreach (var source in _currentMusicSources.Values)
            {
                if (!_musicEnabled)
                {
                    source.GetComponent<AudioSource>().Pause();
                }
                else
                {
                    source.GetComponent<AudioSource>().UnPause();
                }
            }
        }

        private Sound GetSound(string soundName)
        {
            Sound sound;
            if (!_sounds.TryGetValue(soundName, out sound))
            {
                Debug.LogError("Sound '" + soundName + "' doesn't exist");
                return null;
            }

            return sound;
        }

        private AudioSource GetAudioSource(Guid guid)
        {
            GameObject source;
            var isMusic = false;
            if (!_currentSoundFxSources.TryGetValue(guid, out source))
            {
                if (!_currentMusicSources.TryGetValue(guid, out source))
                {
                    Debug.LogError("AudioSource for '" + guid + "' doesn't exist");
                    return null;
                }

                isMusic = true;
            }

            if (!_soundFxEnabled && !isMusic || !_musicEnabled && isMusic)
            {
                return null;
            }

            return source.GetComponent<AudioSource>();
        }

        private GameObject BuildAudioSource(Sound sound)
        {
            var child = new GameObject();
            
            var source = child.AddComponent<AudioSource>();
            source.clip = sound.clip;

            source.volume = sound.volume;
            source.pitch = sound.pitch;
            source.loop = sound.loop;
            source.spatialBlend = sound.spatialBlend;

            return child;
        }
    }
}