using System;
using GLEAMoscopeVR.Environment;
using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.Settings;
using GLEAMoscopeVR.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.Spectrum
{
    /// <summary> 
    /// Used for buttons either side of the slider to change the current wavelength in the chosen direction. 
    /// </summary>
    public class SpectrumButton : MonoBehaviour, IActivatable, IHideableUI
    {
        [SerializeField] private SpectrumDirection direction;

        [Header("IActivatable")]
        [SerializeField] private float activationTime = 2f;
        [SerializeField] private bool canActivate = false;

        [Header("Colours")]
        [SerializeField] private Color activatableColour;
        [SerializeField] private Color inactiveColour = new Color(68, 68, 68);

        [Space] public TextMeshProUGUI ButtonText;

        [Space]
        public PassiveModeRotator Rotator;
        public SunsetController SunsetController;
        
        float IActivatable.ActivationTime => activationTime;
        bool IActivatable.IsActivated => false;
        public CanvasGroup CanvasGroup => _canvasGroup;
        public Collider Collider => _collider;

        #region References
        CanvasGroup _canvasGroup;
        Collider _collider;
        Image _image;
        #endregion

        #region Unity Methods
        void Awake()
        {
            SetAndCheckReferences();
        }

        void OnEnable()
        {
            EventManager.Instance.AddListener<SpectrumStateChangedEvent>(_ => UpdateButtonState());
            EventManager.Instance.AddListener<ExperienceModeChangedEvent>(_ => UpdateButtonState());
        }

        void OnDisable()
        {
            EventManager.Instance.RemoveListener<SpectrumStateChangedEvent>(_ => UpdateButtonState());
            EventManager.Instance.RemoveListener<ExperienceModeChangedEvent>(_ => UpdateButtonState());
        }
        #endregion

        private void UpdateButtonState()
        {
            var wavelength = SpectrumStateController.Instance.CurrentWavelength;
            var mode = SettingsManager.Instance.CurrentExperienceMode;

            if (mode == ExperienceMode.Introduction)
            {
                SetVisibleAndInteractableState(false);
            }
            else
            {
                switch (direction)
                {
                    case SpectrumDirection.Shorter when wavelength != Wavelength.Gamma:
                    case SpectrumDirection.Longer when wavelength != Wavelength.Radio:
                        SetVisibleAndInteractableState(true);
                        break;
                    default:
                        SetVisibleAndInteractableState(true);
                        break;
                }
            }
        }

        private bool CanMoveInDirection()
        {
            var stateCount = Enum.GetValues(typeof(Wavelength)).Length;

            switch (direction)
            {
                case SpectrumDirection.Shorter when ((int)SpectrumStateController.Instance.CurrentWavelength) >= 1:
                case SpectrumDirection.Longer when ((int)SpectrumStateController.Instance.CurrentWavelength) < stateCount - 1:
                    return true;
            }

            return false;
        }

        bool IActivatable.CanActivate()
        {
            return canActivate
                   && CanMoveInDirection()
                   && SettingsManager.Instance.CurrentExperienceMode != ExperienceMode.Introduction
                   && !SpectrumStateController.Instance.IsFading()
                   && Rotator.CanSetRotationTarget()
                   && SunsetController.SunsetCompleted;
        }

        void IActivatable.Activate()
        {
            if (this is IActivatable activatable && activatable.CanActivate())
            {
                switch (direction)
                {
                    case SpectrumDirection.Shorter:
                        SpectrumStateController.Instance.StateBackward();
                        break;
                    case SpectrumDirection.Longer:
                        SpectrumStateController.Instance.StateForward();
                        break;
                }
            }
        }

        public void SetVisibleAndInteractableState(bool visible)
        {
            var validMode = SettingsManager.Instance.CurrentExperienceMode != ExperienceMode.Introduction;
            var interactable = false;

            switch (direction)
            {
                case SpectrumDirection.Shorter when SpectrumStateController.Instance.CurrentWavelength != Wavelength.Gamma:
                case SpectrumDirection.Longer when SpectrumStateController.Instance.CurrentWavelength != Wavelength.Radio:
                    interactable = true;
                    break;
            }

            canActivate = validMode && interactable;
            _canvasGroup.alpha = validMode ? 1 : 0;
            _collider.enabled = validMode && interactable;
            _image.enabled = validMode;
            ButtonText.enabled = validMode;
            _image.color = interactable ? activatableColour : inactiveColour;
            
        }
        
        private void SetAndCheckReferences()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            Assert.IsNotNull(_canvasGroup, $"<b>[{GetType().Name} - {gameObject.name}]</b> has no canvas group component");

            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_collider, $"<b>[{GetType().Name} - {gameObject.name}]</b> has no collider component");

            _image = GetComponent<Image>();
            Assert.IsNotNull(_image, $"<b>[{GetType().Name} - {gameObject.name}]</b> has no image component");

            ButtonText = GetComponentInChildren<TextMeshProUGUI>();
            Assert.IsNotNull(ButtonText, $"<b>[{GetType().Name} - {gameObject.name}]</b> has no text component in children");

            Rotator = FindObjectOfType<PassiveModeRotator>();
            Assert.IsNotNull(Rotator, $"[SpectrumPip] {gameObject.name} cannot find PassiveModeRotator in scene.");

            SunsetController = FindObjectOfType<SunsetController>();
            Assert.IsNotNull(SunsetController, $"[SpectrumPip] {gameObject.name} cannot find SunsetController in scene.");
        }
    }
}