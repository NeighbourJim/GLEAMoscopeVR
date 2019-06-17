using GLEAMoscopeVR.Audio;
using GLEAMoscopeVR.Settings;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Interaction
{
    public class RadioInfoButton : MonoBehaviour, IActivatable, IHideableUI
    {
        public AudioClip RadioAudioClipMale;
        public AudioClip RadioAudioClipFemale;

        [Header("IActivatable")]
        [SerializeField]
        private float activationTime = 2f;
        float IActivatable.ActivationTime => activationTime;
        [SerializeField] private bool isActivated = false;
        bool IActivatable.IsActivated => isActivated;

        CanvasGroup IHideableUI.CanvasGroup => _canvasGroup;
        Collider IHideableUI.Collider => _collider;

        #region References
        CanvasGroup _canvasGroup;
        Collider _collider;
        #endregion

        #region Unity Methods

        void Awake()
        {
            SetAndCheckReferences();
        }
        
        #endregion

        bool IActivatable.CanActivate()//Todo
        {
            return false;
        }

        void IActivatable.Activate()
        {
            //var voiceSetting = FindObjectOfType<SettingsManager>().UserSettings.VoiceSetting;
            //var clip = voiceSetting == VoiceoverSetting.Female ? RadioAudioClipFemale : RadioAudioClipMale;

            //FindObjectOfType<VoiceOverController>().RequestClipPlay(clip);
        }

        void IActivatable.Deactivate() {}

        public void SetVisibleAndInteractableState(bool hidden)
        {
            _canvasGroup.alpha = hidden ? 0 : 1;
            _collider.enabled = !hidden;
        }

        #region Debugging
        private void SetAndCheckReferences()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            Assert.IsNotNull(_canvasGroup, $"<b>[RadioInfoButton]</b> has no Canvas Group component.");

            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_collider, $"<b>[RadioInfoButton]</b> has no Collider component.");

            Assert.IsNotNull(RadioAudioClipMale, $"<b>[RadioInfoButton]</b> Radio audio clip (male) has not been assigned.");
            Assert.IsNotNull(RadioAudioClipMale, $"<b>[RadioInfoButton]</b> Radio audio clip (female) has not been assigned.");
        }
        #endregion
    }
}