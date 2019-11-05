using System;
using UnityEngine;

namespace GLEAMoscopeVR.Audio
{
    [Serializable]
    public class SoundEffect
    {
        public string Name;
        public AudioClip AudioClip;

        [Range(0f, 1f)]
        public float Volume = 0.5f;

        [Range(0.5f, 1.5f)]
        public float Pitch = 1f;

        [HideInInspector]
        public AudioSource AudioSource;
    }
}