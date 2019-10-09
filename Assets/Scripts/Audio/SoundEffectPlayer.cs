using System;
using GLEAMoscopeVR.Utility.Extensions;
using UnityEngine;

namespace GLEAMoscopeVR.Audio
{
    /// <summary>
    /// Stores audio clips in an array, attaches an Audio Source to the game object and uses it to play the clip when <see cref="PlaySoundEffect"/> is invoked.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundEffectPlayer : MonoBehaviour
    {
        public SoundEffect[] SoundEffects;

        [SerializeField] private float defaultVolume = 0.5f;
        [SerializeField] private float defaultPitch = 1f;
        private bool playOnAwake = false;
        private bool loop = false;

        AudioSource _audioSource;

        void Start()
        {
            PrepareAudioSource();
            PrepareSoundEffects();
        }

        private void PrepareAudioSource()
        {
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }

            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }

            _audioSource.loop = false;
            _audioSource.playOnAwake = playOnAwake;
            _audioSource.volume = defaultVolume;
            _audioSource.pitch = defaultPitch;
        }

        private void PrepareSoundEffects()
        {
            if (SoundEffects.IsNullOrEmpty()) return;

            foreach (SoundEffect soundEffect in SoundEffects)
            {
                soundEffect.AudioSource = _audioSource;

                soundEffect.AudioSource.clip = soundEffect.AudioClip;
                soundEffect.AudioSource.volume = soundEffect.Volume;
                soundEffect.AudioSource.pitch = soundEffect.Pitch;
            }
        }

        public void PlaySoundEffect(string effectName)
        {
            SoundEffect s = Array.Find(SoundEffects, sound => sound.Name == effectName);
            s.AudioSource.PlayOneShot(s.AudioClip, 0.7f);
        }
    }
}