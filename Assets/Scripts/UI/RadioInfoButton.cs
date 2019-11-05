using GLEAMoscopeVR.Audio;
using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Settings;
using GLEAMoscopeVR.Spectrum;
using GLEAMoscopeVR.SubtitleSystem;
using GLEAMoscopeVR.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.Interaction
{
    public class RadioInfoButton : MonoBehaviour, IActivatable, IHideableUI
    {
        [Header("Audio")]
        public AudioClip RadioAudioClipFemale;
        public AudioClip RadioAudioClipMale;

        [Header("Subtitles")]
        public SubtitleData RadioInfoSubtitle;

        [Header("IActivatable")]
        [SerializeField] private float activationTime = 1f;
        [SerializeField] private bool isActivated = false;

        [Space]
        public SubtitleDisplayer SubtitleDisplayer;
        
        bool isValidMode = false;
        bool isRadioSky = false;

        float IActivatable.ActivationTime => activationTime;
        bool IActivatable.IsActivated => isActivated;
        CanvasGroup IHideableUI.CanvasGroup => null;
        Collider IHideableUI.Collider => _collider;

        #region References
        Collider _collider;
        Image _image;
        TextMeshProUGUI _text;
        #endregion

        #region Unity Methods
        void Awake()
        {
            SetAndCheckReferences();
            SetVisibleAndInteractableState(false);
        }

        void OnEnable()
        {
            EventManager.Instance.AddListener<SpectrumStateChangedEvent>(_=> DisplayButtonIfAppropriate());
            EventManager.Instance.AddListener<ExperienceModeChangedEvent>(_ => DisplayButtonIfAppropriate());
        }

        void OnDisable()
        {
            EventManager.Instance.RemoveListener<SpectrumStateChangedEvent>(_ => DisplayButtonIfAppropriate());
            EventManager.Instance.RemoveListener<ExperienceModeChangedEvent>(_ => DisplayButtonIfAppropriate());
        }
        #endregion

        bool IActivatable.CanActivate()
        {
            return SettingsManager.Instance.CurrentExperienceMode != ExperienceMode.Introduction 
                && SpectrumStateController.Instance.CurrentWavelength == Wavelength.Radio;
        }

        void IActivatable.Activate()
        {
            if (SettingsManager.Instance.CurrentVoiceoverSetting != VoiceoverSetting.None)
            {
                var clip = SettingsManager.Instance.CurrentVoiceoverSetting == VoiceoverSetting.Female 
                    ? RadioAudioClipFemale 
                    : RadioAudioClipMale;
                VoiceoverController.Instance.RequestClipPlay(clip);
            }
            
            if (SettingsManager.Instance.ShowSubtitles)
            {
                SubtitleDisplayer.SetSubtitleQueue(RadioInfoSubtitle);
            }
        }
        
        private void DisplayButtonIfAppropriate()
        {
            isRadioSky = SpectrumStateController.Instance.CurrentWavelength == Wavelength.Radio;
            isValidMode = SettingsManager.Instance.CurrentExperienceMode != ExperienceMode.Introduction;

            SetVisibleAndInteractableState(isRadioSky && isValidMode);
        }

        public void SetVisibleAndInteractableState(bool visible)
        {
            _collider.enabled = visible;
            _image.enabled = visible;
            _text.enabled = visible;
        }

        private void SetAndCheckReferences()
        {
            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_collider, $"<b>[{GetType().Name}]</b> has no Collider component.");

            _image = GetComponent<Image>();
            Assert.IsNotNull(_image, $"<b>[{GetType().Name}]</b> has no Image component.");

            _text = GetComponentInChildren<TextMeshProUGUI>();
            Assert.IsNotNull(_text, $"<b>[{GetType().Name}]</b> has no Text component in children.");

            Assert.IsNotNull(RadioAudioClipMale, $"<b>[{GetType().Name}]</b> Radio audio clip (male) has not been assigned.");
            Assert.IsNotNull(RadioAudioClipMale, $"<b>[{GetType().Name}]</b> Radio audio clip (female) has not been assigned.");
            Assert.IsNotNull(RadioInfoSubtitle, $"<b>[{GetType().Name}]</b> Radio info subtitles is not assigned.");

            if (SubtitleDisplayer == null)
            {
                SubtitleDisplayer = FindObjectOfType<SubtitleDisplayer>();
            }
            Assert.IsNotNull(SubtitleDisplayer, $"<b>[{GetType().Name}]</b> Subtitle displayer is not assigned and cannot be found in scene.");
        }
    }
}