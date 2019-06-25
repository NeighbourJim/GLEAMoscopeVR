using GLEAMoscopeVR.Menu;
using GLEAMoscopeVR.SubtitleSystem;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Video;

namespace GLEAMoscopeVR.Interaction
{
    public class StartButton : MonoBehaviour, IActivatable, IHideableUI
    {
        [SerializeField] private float activationTime = 2f;
        [SerializeField] private bool canActivate = false;
        
        #region References
        CanvasGroup _canvasGroup;
        Collider _collider;

        PlayVideo _playVideoScript;
        VideoPlayer _videoPlayer;
        StartScreenManager _startManager;
        SoundEffects _soundEffects;
        Subtitle _subtitle;
        #endregion

        CanvasGroup IHideableUI.CanvasGroup => _canvasGroup;
        Collider IHideableUI.Collider => _collider;

        float IActivatable.ActivationTime => activationTime;
        bool IActivatable.IsActivated => false;

        #region Unity Methods

        void Awake()
        {
            SetAndCheckReferences();
            //SetVisibleAndInteractableState(false);
        }

        void Start()
        {
            _playVideoScript.OnVideoFinished += HandleVideoFinished;
        }

        #endregion

        private void HandleVideoFinished()
        {
            canActivate = true;
            //SetVisibleAndInteractableState(true);
        }

        bool IActivatable.CanActivate()
        {
            return canActivate;
        }

        void IActivatable.Activate()
        {
            _startManager.StartTeleport();
            _subtitle.SendSubtitle();
            SetVisibleAndInteractableState(false);
            canActivate = false;
            _canvasGroup.gameObject.GetComponent<SmoothRotate>().enabled = false;
        }

        void IActivatable.Deactivate(){}

        public void SetVisibleAndInteractableState(bool visible)
        {
            _canvasGroup.alpha = visible ? 1 : 0;
            _collider.enabled = visible;
        }

        #region Debugging
        private void SetAndCheckReferences()
        {
            _canvasGroup = GetComponentInParent<CanvasGroup>();
            Assert.IsNotNull(_canvasGroup, $"<b>[StartButton]</b> has no Canvas Group component in parent.");

            _collider = GetComponentInParent<Collider>();
            Assert.IsNotNull(_collider, $"<b>[StartButton]</b> has no collider component.");

            _playVideoScript = FindObjectOfType<PlayVideo>();
            Assert.IsNotNull(_playVideoScript, $"[StartButton] {gameObject.name} cannot find PlayVideo script in scene.");

            _videoPlayer = FindObjectOfType<PlayVideo>().GetComponent<VideoPlayer>();
            Assert.IsNotNull(_videoPlayer, $"[StartButton] {gameObject.name} cannot find VideoPlayer component on PlayVideo script game object.");
            
            _startManager = FindObjectOfType<StartScreenManager>();
            Assert.IsNotNull(_startManager, $"[StartButton] {gameObject.name} cannot find StartScreenManager in the scene.");

            _subtitle = gameObject.GetComponent<Subtitle>();
            Assert.IsNotNull(_subtitle, $"[StartButton] {gameObject.name} cannot find Subtitle component on StartButton script game object.");
        }
        #endregion
    }
}