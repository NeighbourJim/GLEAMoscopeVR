using GLEAMoscopeVR.Interaction;
using System.Collections;
using GLEAMoscopeVR.Events;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Video;

namespace GLEAMoscopeVR.Menu
{
    /// <summary> 
    /// Used for playing ICRAR's logo video when the app starts. 
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(VideoPlayer))]
    public class PlayIntroVideo : MonoBehaviour
    {
        public VideoClip VideoClip = null;
        public AudioClip AudioClip = null;

        [Space, SerializeField]
        private bool hasSeparateAudio = true;

        [Space] public CameraRayCaster CameraRayCaster;

        #region References
        AudioSource _audioSource;
        Renderer _renderer;
        VideoPlayer _videoPlayer;
        #endregion

        #region Unity Methods
        void Awake()
        {
            SetAndCheckReferences();
            SetIntialState();

            StartCoroutine(PlayRoutine());
        }
        #endregion

        #region Component Settings
        private void SetIntialState()
        {
            SetVideoPlayerSettings();
            SetAudioSourceSettings();
        }
        
        private void SetVideoPlayerSettings()
        {
            _videoPlayer.clip = VideoClip;
            _videoPlayer.targetCamera = Camera.main;
            _videoPlayer.playOnAwake = false;
            _videoPlayer.isLooping = false;
        }

        private void SetAudioSourceSettings()
        {
            _audioSource.clip = AudioClip;
            _audioSource.loop = false;
            _audioSource.playOnAwake = false;
        }
        #endregion

        #region Player Behaviour
        private void Play()
        {
            _videoPlayer.Play();

            if (hasSeparateAudio)
            {
                _audioSource.Play();
            }
        }

        /// <summary>
        /// Prepares the video, plays it and the accompanying audio (if applicable).
        /// Once the video finishes, the game object is deactivated.
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayRoutine()
        {
            _videoPlayer.Prepare();
            yield return new WaitUntil(() => _videoPlayer.isPrepared);
            
            _renderer.enabled = true;
            CameraRayCaster.reticleAlpha = 0;

            Play();

            yield return new WaitUntil(() => !_videoPlayer.isPlaying);

            UpdateComponentState();
            EventManager.Instance.Raise(new VideoClipEndedEvent($"Intro video complete."));
            yield break;
        }

        private void UpdateComponentState()
        {
            GvrCardboardHelpers.Recenter();

            _renderer.enabled = false;

            CameraRayCaster.reticleAlpha = CameraRayCaster.reticleStartingAlpha;
            
            gameObject.SetActive(false);
        }

        #endregion
        
        private void SetAndCheckReferences()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
            Assert.IsNotNull(_videoPlayer, $"<b>[{GetType().Name}]</b> does not have a VideoPlayer component.");

            _audioSource = GetComponent<AudioSource>();
            Assert.IsNotNull(_audioSource, $"<b>[{GetType().Name}]</b> does not have a AudioSource component.");

            _renderer = GetComponent<Renderer>();
            Assert.IsNotNull(_renderer, $"<b>[{GetType().Name}]</b> does not have a Renderer component.");

            if (CameraRayCaster == null)
            {
                CameraRayCaster = Camera.main.gameObject.GetComponent<CameraRayCaster>();
            }
            Assert.IsNotNull(CameraRayCaster, $"<b>[{GetType().Name}]</b> Raycast script is not assigned and cannot be found in scene.");

            Assert.IsNotNull(VideoClip, $"<b>[{GetType().Name}]</b> Video clip has not been assigned.");

            if (hasSeparateAudio)
            {
                Assert.IsNotNull(AudioClip, $"<b>[{GetType().Name}]</b> audio clip has not been assigned. If the video clip includes audio, set hasSeparateAudio to false.");
            }
        }
    }
}