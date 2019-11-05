using GLEAMoscopeVR.Environment;
using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.Settings;
using GLEAMoscopeVR.Utility.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.Spectrum
{
    public class SpectrumPip : MonoBehaviour, IActivatable
    {
        [SerializeField] private Wavelength wavelength;

        [Space] public TextMeshProUGUI Label;

        [Header("IActivatable")]
        [SerializeField] private float activationTime = 1f;
        
        [Header("Colours")]
        [SerializeField] private Color activatableColour;
        [SerializeField] private Color inactiveColour = new Color(68, 68, 68);

        [Space]
        public PassiveModeRotator Rotator;
        public SunsetController SunsetController;
        SpectrumStateController spectrumStateController;

        public Wavelength Wavelength => wavelength;
        public int WavelengthIndex => (int)wavelength;
        float IActivatable.ActivationTime => activationTime;
        bool IActivatable.IsActivated => false;

        #region References
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
            EventManager.Instance.AddListener<ExperienceModeChangedEvent>(HandleExperienceModeChanged);
        }

        void OnDisable()
        {
            EventManager.Instance.RemoveListener<ExperienceModeChangedEvent>(HandleExperienceModeChanged);
        }
        #endregion

        private void HandleExperienceModeChanged(ExperienceModeChangedEvent e)
        {
            ToggleActivatableStatus(e.ExperienceMode != ExperienceMode.Introduction);
        }

        public bool CanActivate()
        {
            return wavelength != SpectrumStateController.Instance.CurrentWavelength
                   && SunsetController.SunsetCompleted
                   && Rotator.CanSetRotationTarget();
        }

        void IActivatable.Activate()
        {
            if (CanActivate())
            {
                spectrumStateController.StateQuick((int)wavelength);
            }
        }
        
        public void ToggleActivatableStatus(bool isActivatable)
        {
            _collider.enabled = isActivatable;
            _image.color = isActivatable ? activatableColour : inactiveColour;
        }
        
        private void SetAndCheckReferences()
        {
            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_collider, $"<b>[{GetType().Name} - {gameObject.name}]</b> has no collider component");

            _image = GetComponent<Image>();
            Assert.IsNotNull(_image, $"<b>[{GetType().Name} - {gameObject.name}]</b> has no image component");

            Rotator = FindObjectOfType<PassiveModeRotator>();
            Assert.IsNotNull(Rotator, $"<b>[{wavelength} {GetType().Name}]<b> cannot find PassiveModeRotator in scene.");

            SunsetController = FindObjectOfType<SunsetController>();
            Assert.IsNotNull(SunsetController, $"<b>[{wavelength} {GetType().Name}]<b> cannot find SunsetController in scene.");

            if (Label == null)
            {
                Label = GetComponentInChildren<TextMeshProUGUI>();
            }
            Assert.IsNotNull(Label, $"<b>[{wavelength} {GetType().Name}]<b> label is not assigned and cannot be found in children.");
            Label.text = wavelength.GetDescription();

            spectrumStateController = SpectrumStateController.Instance;
        }
    }
}
