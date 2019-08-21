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

        private IDictionary<Guid, GameObject> _currentSources;

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
            
            _currentSources = new Dictionary<Guid, GameObject>();
            
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
            _currentSources.Add(guid, source);
            source.transform.parent = gameObject.transform;

            return guid;
        }

        public void Play(Guid guid)
        {
            GetAudioSource(guid).Play();
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
            if (!_currentSources.TryGetValue(guid, out source))
            {
                return;
            }
            
            Destroy(source);
            _currentSources.Remove(guid);
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
            if (!_currentSources.TryGetValue(guid, out source))
            {
                Debug.LogError("AudioSource for '" + guid + "' doesn't exist");
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