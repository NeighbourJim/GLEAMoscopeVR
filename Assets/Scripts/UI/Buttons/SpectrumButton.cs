using System;
using GLEAMoscopeVR.POIs;
using GLEAMoscopeVR.Settings;
using GLEAMoscopeVR.Spectrum;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Interaction
{
    /// <summary> Used for buttons either side of the slider to change the current wavelength in the chosen direction. </summary>
    public class SpectrumButton : MonoBehaviour, IActivatable
    {
        private const string soundEffect = "Click";

        [SerializeField]
        private SpectrumDirection direction;

        [Header("IActivatable")]
        [SerializeField]
        private float activationTime = 2f;

        float IActivatable.ActivationTime => activationTime;
        bool IActivatable.IsActivated => false;
        
        #region References
        ExperienceModeController _modeController;
        WavelengthStateController _wavelengthController;
        PassiveModeRotator _rotator;
        SoundEffects _soundEffects;
        SunsetController _sunsetController;
        #endregion

        #region Unity Methods
        void Start()
        {
            SetAndCheckReferences();
        }
        #endregion

        bool IActivatable.CanActivate()
        {
            var canMove = CanMoveInDirection();

            return canMove 
                   && _modeController.CurrentMode != ExperienceMode.Introduction
                   && !_wavelengthController.IsFading()
                   && _rotator.CanSetRotationTarget()
                   && _sunsetController.SunsetCompleted;
        }

        private bool CanMoveInDirection()
        {
            var canMove = false;
            var stateCount = Enum.GetValues(typeof(Wavelengths)).Length;

            switch (direction)
            {
                case SpectrumDirection.Shorter when ((int) _wavelengthController.CurrentWavelength) >= 1:
                case SpectrumDirection.Longer when ((int) _wavelengthController.CurrentWavelength) < stateCount - 1:
                    canMove = true;
                    break;
            }

            return canMove;
        }

        void IActivatable.Activate()
        {
            if (this is IActivatable activatable && activatable.CanActivate())
            {
                switch (direction)
                {
                    case SpectrumDirection.Shorter:
                        _wavelengthController.StateBackward();
                        break;
                    case SpectrumDirection.Longer:
                        _wavelengthController.StateForward();
                        break;
                }

                _soundEffects.Play(soundEffect);
            }
        }

       void IActivatable.Deactivate() { }

        #region Debugging
        private void SetAndCheckReferences()
        {
            _wavelengthController = WavelengthStateController.Instance;
            Assert.IsNotNull(_wavelengthController, $"[SpectrumButton] {gameObject.name} cannot find WavelengthStateController in scene.");

            _soundEffects = FindObjectOfType<SoundEffects>();
            Assert.IsNotNull(_soundEffects, $"[SpectrumButton] {gameObject.name} cannot find SoundEffects in scene.");

            _modeController = FindObjectOfType<ExperienceModeController>().Instance;
            Assert.IsNotNull(_modeController, $"[SpectrumButton] {gameObject.name} cannot find ExperienceModeController in scene.");

            _rotator = FindObjectOfType<PassiveModeRotator>();
            Assert.IsNotNull(_rotator, $"[SpectrumPip] {gameObject.name} cannot find PassiveModeRotator in scene.");

            _sunsetController = FindObjectOfType<SunsetController>();
            Assert.IsNotNull(_sunsetController, $"[SpectrumPip] {gameObject.name} cannot find SunsetController in scene.");
        }
        #endregion
    }
}