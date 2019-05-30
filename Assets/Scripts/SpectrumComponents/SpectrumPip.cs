using GLEAMoscopeVR.POIs;
using GLEAMoscopeVR.Settings;
using GLEAMoscopeVR.Spectrum;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Interaction
{
    /// <summary> PURPOSE OF SCRIPT GOES HERE </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class SpectrumPip : MonoBehaviour, IActivatable
    {
        private const string soundEffectClip = "WaveLengthJump";

        [SerializeField]
        private Wavelengths wavelength;
        public int WavelengthIndex => (int) wavelength;

        [SerializeField]
        private float activationTime = 1f;

        float IActivatable.ActivationTime => activationTime;

        bool IActivatable.IsActivated => false;
        
        #region References
        WavelengthStateController _wavelengthController;
        ExperienceModeController _modeController;
        PassiveModeRotator _rotator;
        SoundEffects _soundEffects;
        SunsetController _sunsetController;
        #endregion

        #region Unity Methods
        void Awake() {}

        void Start()
        {
            SetAndCheckReferences();
        }
        
        #endregion

        public bool CanActivate()
        {
            return wavelength != _wavelengthController.CurrentWavelength
                   && _modeController.CurrentMode != ExperienceMode.Introduction
                   && _sunsetController.SunsetCompleted
                   && _rotator.CanSetRotationTarget();
        }

        void IActivatable.Activate()
        {
            if (CanActivate())
            {
                _wavelengthController.StateQuick((int) wavelength);
                _soundEffects.Play("WaveLengthJump");
            }
        }

        void IActivatable.Deactivate(){}
        
        #region Debugging
        private void SetAndCheckReferences()
        {
            _wavelengthController = WavelengthStateController.Instance;
            Assert.IsNotNull(_wavelengthController, $"[SpectrumPip] {gameObject.name} cannot find WavelengthStateController in scene.");

            _modeController = FindObjectOfType<ExperienceModeController>().Instance;
            Assert.IsNotNull(_modeController, $"[SpectrumPip] {gameObject.name} cannot find ExperienceModeController in scene.");

            _rotator = FindObjectOfType<PassiveModeRotator>();
            Assert.IsNotNull(_rotator, $"[SpectrumPip] {gameObject.name} cannot find PassiveModeRotator in scene.");

            _soundEffects = FindObjectOfType<SoundEffects>();
            Assert.IsNotNull(_soundEffects, $"[SpectrumPip] {gameObject.name} cannot find SoundEffects in scene.");

            _sunsetController = FindObjectOfType<SunsetController>();
            Assert.IsNotNull(_sunsetController, $"[SpectrumPip] {gameObject.name} cannot find SunsetController in scene.");
        }
        #endregion
    }
}