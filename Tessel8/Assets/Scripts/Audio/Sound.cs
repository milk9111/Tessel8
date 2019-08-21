using System;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public class Sound
    {
        public string name;
        
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        [Range(0f, 1f)] 
        public float spatialBlend;

        public bool loop;

        [HideInInspector]
        public AudioSource source;
    }
}