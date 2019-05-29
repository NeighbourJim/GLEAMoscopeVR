using GLEAMoscopeVR.RaycastingSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Video;

namespace GLEAMoscopeVR.Menu
{
    /// <summary> Used for playing ICRARs logo animation video when the app starts. </summary>
    [RequireComponent(typeof(VideoPlayer))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Renderer))]
    public class PlayVideo : MonoBehaviour
    {
        [SerializeField] private VideoClip videoClip = null;
        [SerializeField] private AudioClip audioClip = null;
        [SerializeField] private bool hasSeparateAudio = true;

        public CanvasGroup MenuCanvasGroup;

        #region References
        AudioSource _audioSource;
        Renderer _renderer;
        VideoPlayer _videoPlayer;
        Script_CameraRayCaster _rayCastScript;
        #endregion

        #region Unity Methods

        void Awake()
        {
            SetAndCheckReferences();
            SetVideoPlayerSettings();
            SetAudioSourceSettings();
            ShowMenuCanvasGroup(false);
            StartCoroutine(PlayRoutine());
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
        IEnumerator PlayRoutine()
        {
            _videoPlayer.Prepare();
            yield return new WaitUntil(() => _videoPlayer.isPrepared);
            
            _renderer.enabled = true;
            _rayCastScript.reticleAlpha = 0;

            Play();

            yield return new WaitUntil(() => !_videoPlayer.isPlaying);

            _rayCastScript.reticleAlpha = _rayCastScript.reticleStartingAlpha;
            _renderer.enabled = false;
            gameObject.SetActive(false);
            ShowMenuCanvasGroup(true);
            yield break;
        }
        #endregion

        #region Component Settings
        private void SetVideoPlayerSettings()
        {
            _videoPlayer.clip = videoClip;
            _videoPlayer.targetCamera = Camera.main;
            _videoPlayer.playOnAwake = false;
            _videoPlayer.isLooping = false;
            //_videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        }

        private void SetAudioSourceSettings()
        {
            _audioSource.clip = audioClip;
            _audioSource.loop = false;
            _audioSource.playOnAwake = false;
        }
        #endregion

        #region UI
        private void ShowMenuCanvasGroup(bool show)
        {
            MenuCanvasGroup.alpha = show ? 1 : 0;
            MenuCanvasGroup.interactable = show;
            MenuCanvasGroup.blocksRaycasts = show;
        }
        #endregion
        
        #region Debugging

        private void SetAndCheckReferences()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
            Assert.IsNotNull(_videoPlayer, $"[PlayVideo] {gameObject.name} does not have a VideoPlayer component.");

            _audioSource = GetComponent<AudioSource>();
            Assert.IsNotNull(_audioSource, $"[PlayVideo] {gameObject.name} does not have a AudioSource component.");

            _renderer = GetComponent<Renderer>();
            Assert.IsNotNull(_renderer, $"[PlayVideo] {gameObject.name} does not have a Renderer component.");

            _rayCastScript = GetComponentInParent<Script_CameraRayCaster>();
            Assert.IsNotNull(_renderer, $"[PlayVideo] {gameObject.name} does not have a Script_CameraRayCaster component.");

            Assert.IsNotNull(videoClip, $"[PlayVideo] {gameObject.name} video clip has not been assigned.");
            Assert.IsNotNull(MenuCanvasGroup, $"[PlayVideo] {gameObject.name} Start Screen UI Canvas group not assigned.");

            if(hasSeparateAudio)
            {
                Assert.IsNotNull(audioClip, $"[PlayVideo] {gameObject.name} audio clip has not been assigned. If the video clip includes audio, set hasSeparateAudio to false.");
            }
        }

        #endregion
    }
}