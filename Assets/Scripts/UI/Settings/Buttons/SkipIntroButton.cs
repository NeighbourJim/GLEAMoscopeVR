using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Settings;
using GLEAMoscopeVR.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.Interaction
{
    public class SkipIntroButton : MonoBehaviour, IActivatable, IHideableUI
    {
        [Header("IActivatable")]
        [SerializeField] private float activationTime = 3f;
        [SerializeField] private bool canActivate = false;

        [Space] public IntroductionSequenceController IntroductionSequenceController;

        CanvasGroup IHideableUI.CanvasGroup => null;
        Collider IHideableUI.Collider => _collider;
        float IActivatable.ActivationTime => activationTime;
        bool IActivatable.IsActivated => false;
        
        #region References
        Collider _collider;
        Image _image;
        TextMeshProUGUI _text;
        #endregion

        #region Unity Methods
        void Awake()
        {
            SetAndCheckComponentReferences();
            SetVisibleAndInteractableState(false);
        }

        void OnEnable()
        {
            EventManager.Instance.AddListener<ExperienceModeChangedEvent>(HandleExperienceModeChanged);
            EventManager.Instance.AddListener<IntroductionSequenceStateChangedEvent>(HandleIntroductionStateChanged);
        }

        void OnDisable()
        {
            EventManager.Instance.AddListener<ExperienceModeChangedEvent>(HandleExperienceModeChanged);
            EventManager.Instance.RemoveListener<IntroductionSequenceStateChangedEvent>(HandleIntroductionStateChanged);
        }
        #endregion

        bool IActivatable.CanActivate()
        {
            return SettingsManager.Instance.CurrentExperienceMode == ExperienceMode.Introduction && canActivate;
        }

        void IActivatable.Activate()
        {
            if (IntroductionSequenceController.IsPlaying)
            {
                IntroductionSequenceController.StopIntroductionSequenece();
                SetVisibleAndInteractableState(false);
            }
        }

        private void HandleExperienceModeChanged(ExperienceModeChangedEvent e)
        {
            if (e.ExperienceMode != ExperienceMode.Introduction)
            {
                SetVisibleAndInteractableState(false);
            }
        }

        private void HandleIntroductionStateChanged(IntroductionSequenceStateChangedEvent e)
        {
            SetVisibleAndInteractableState(e.State == IntroductionSequenceStateChangedEvent.IntroState.Playing);
        }

        public void SetVisibleAndInteractableState(bool visible)
        {
            canActivate = visible;
            _collider.enabled = visible;
            _image.enabled = visible;
            _text.enabled = visible;
        }
        
        private void SetAndCheckComponentReferences()
        {
            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_collider, $"<b>[{GetType().Name}]</b> has no Collider component.");

            _image = GetComponent<Image>();
            Assert.IsNotNull(_image, $"<b>[{GetType().Name}]</b> has no Image component.");

            _text = GetComponentInChildren<TextMeshProUGUI>();
            Assert.IsNotNull(_text, $"<b>[{GetType().Name}]</b> has no TextMeshProUGUI component in children.");

            if (IntroductionSequenceController == null)
            {
                IntroductionSequenceController = FindObjectOfType<IntroductionSequenceController>();
            }
            Assert.IsNotNull(IntroductionSequenceController, $"<b>[{GetType().Name}]</b> Intro sequence controller is not assigned and cannot be found in the scene.");
        }
    }
}