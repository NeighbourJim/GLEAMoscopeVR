﻿using GLEAMoscopeVR.Utility.Management;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Audio
{
    /// <summary>
    /// Audio source that fades between clips instead of playing them immediately.
    /// Adapted from https://wiki.unity3d.com/index.php/Fading_Audio_Source
    /// Todo: revisit if we need to handle fading and delays at the same time. Works for now.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class VoiceOverController : GenericSingleton<VoiceOverController>
    {
        public enum FadeState
        {
            None,
            FadingOut,
            FadingIn
        }

        /// <summary> Volume to end the previous clip at. </summary>
        [Tooltip("Volume to end the previous clip at.")]
        public float FadeOutThreshold = 0.4f;

        /// <summary> Volume change per second when fading. </summary>
        [Tooltip("Volume change per second when fading.")]
        public float FadeSpeed = 0.5f;

        /// <summary> Whether the audio source is currently fading, in or out. </summary>
        private FadeState fadeState = FadeState.None;

        /// <summary> Next clip to fade to. </summary>
        private AudioClip fadeInClip;

        /// <summary> Target volume to fade the next clip to. </summary>
        private float targetVolume;

        /// <summary> Current clip of the audio source. </summary>
        public AudioClip Clip => _audioSource.clip;

        /// <summary> Whether the audio source is currently playing a clip. </summary>
        public bool IsPlaying => _audioSource.isPlaying;

        /// <summary> Current volume of the audio source. </summary>
        public float Volume => _audioSource.volume;

        #region References
        private AudioSource _audioSource;
        #endregion

        #region Unity Methods
        protected override void Awake()
        {
            base.Awake();
            CheckAndSetReferences();
        }

        private void Update()
        {
            PerformFadeBehaviour();
        }
        #endregion

        /// <summary>
        /// If the audio source is enabled and playing, fades out the current clip and fades in the specified one, after.
        /// If the audio source is enabled and not playing, fades in the specified clip immediately.
        /// If the audio source is not enabled, fades in the specified clip as soon as it gets enabled.
        /// </summary>
        /// <param name="clip">Clip to fade in.</param>
        /// <param name="delaySeconds">Time, in seconds, to wait before playing the audio clip.</param>
        public void RequestClipPlay(AudioClip clip, float delaySeconds = 0)
        {
            if (clip == null || clip == _audioSource.clip) return;

            fadeInClip = clip;
            targetVolume = _audioSource.volume;

            if (IsPlaying)
            {
                fadeState = FadeState.FadingOut;
            }
            else
            {
                FadeToNextClip(delaySeconds);
            }
        }

        private void PerformFadeBehaviour()
        {
            if (fadeState == FadeState.FadingOut)
            {
                if (_audioSource.volume > FadeOutThreshold)
                {
                    // RequestClipPlay out current clip.
                    _audioSource.volume -= FadeSpeed * Time.deltaTime;
                }
                else
                {
                    // Start fading in next clip.
                    FadeToNextClip();
                }
            }
            else if (fadeState == FadeState.FadingIn)
            {
                if (_audioSource.volume < targetVolume)
                {
                    // RequestClipPlay in next clip.
                    _audioSource.volume += FadeSpeed * Time.deltaTime;
                }
                else
                {
                    // Stop fading in.
                    fadeState = FadeState.None;
                }
            }
        }

        private void FadeToNextClip(float delaySeconds = 0)
        {
            _audioSource.clip = fadeInClip;

            fadeState = FadeState.FadingIn;

            if (_audioSource.enabled)
            {
                _audioSource.PlayDelayed(delaySeconds);
            }
        }

        #region Debugging
        private void CheckAndSetReferences()
        {
            _audioSource = GetComponent<AudioSource>();
            Assert.IsNotNull(_audioSource, $"[VoiceOverController] has no AudioSource component.");
            targetVolume = _audioSource.volume;
        }
        #endregion
    }
}