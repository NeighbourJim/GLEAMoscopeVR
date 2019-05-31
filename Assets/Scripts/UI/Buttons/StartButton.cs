using GLEAMoscopeVR.Menu;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Video;

namespace GLEAMoscopeVR.Interaction
{
    /// <summary> PURPOSE OF SCRIPT GOES HERE </summary>
    public class StartButton : MonoBehaviour, IActivatable
    {
        [SerializeField] private float activationTime = 2f;
        [SerializeField] private bool canActivate = false;

        float IActivatable.ActivationTime => activationTime;

        bool IActivatable.IsActivated => false;

        #region References
        PlayVideo _playVideoScript;
        VideoPlayer _videoPlayer;
        StartScreenManager _startManager;
        SoundEffects _soundEffects;
        #endregion

        #region Unity Methods

        void Start()
        {
            SetAndCheckReferences();
            _playVideoScript.OnVideoFinished += HandleVideoFinished;
        }

        #endregion

        private void HandleVideoFinished()
        {
            canActivate = true;
        }

        bool IActivatable.CanActivate()
        {
            return canActivate;
        }

        void IActivatable.Activate()
        {
            _startManager.StartTeleport();
        }

        void IActivatable.Deactivate(){}


        #region Debugging
        private void SetAndCheckReferences()
        {
            _playVideoScript = FindObjectOfType<PlayVideo>();
            Assert.IsNotNull(_playVideoScript, $"[StartButton] {gameObject.name} cannot find PlayVideo script in scene.");

            _videoPlayer = FindObjectOfType<PlayVideo>().GetComponent<VideoPlayer>();
            Assert.IsNotNull(_videoPlayer, $"[StartButton] {gameObject.name} cannot find VideoPlayer component on PlayVideo script game object.");
            
            _startManager = FindObjectOfType<StartScreenManager>();
            Assert.IsNotNull(_startManager, $"[StartButton] {gameObject.name} cannot find StartScreenManager in the scene.");
        }

        #endregion
    }
}