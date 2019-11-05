using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.POIs;
using GLEAMoscopeVR.Spectrum;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.Interaction
{
    /// <summary>
    /// Provide more control over sprite cycling to assist with POIs that don't exist in more than one sprite (Antenna)
    /// </summary>
    public class SwapSpriteButton : MonoBehaviour, IActivatable
    {
        public InfoPanelBase InfoPanel;

        [Space]
        [SerializeField] private SpectrumDirection direction;
        [SerializeField] private Wavelength spriteWavelength;

        [Header("IActivatable")]
        [SerializeField] private float activationTime = 1f;
        [SerializeField] private bool isActivated = false;
        [SerializeField] private bool canActivate = false;
        [SerializeField] private bool canMoveInDirection = false;

        [Header("Colours")]
        [SerializeField] private Color activatableColour;
        [SerializeField] private Color inactiveColour = new Color(68, 68, 68);

        float IActivatable.ActivationTime => activationTime;
        bool IActivatable.IsActivated => isActivated;

        #region References
        Image _image;
        Collider _collider;
        #endregion

        #region Unity Methods
        void Awake()
        {
            SetAndCheckReferences();
        }

        void Start()
        {
            spriteWavelength = SpectrumStateController.Instance.CurrentWavelength;
        }

        void OnEnable()
        {
            EventManager.Instance.AddListener<SpriteWavelengthChangedEvent>(HandleSpriteWavelengthChanged);
        }
        void OnDisable()
        {
            EventManager.Instance.RemoveListener<SpriteWavelengthChangedEvent>(HandleSpriteWavelengthChanged);
        }
        #endregion
        
        bool IActivatable.CanActivate()
        {
            switch (direction)
            {
                case SpectrumDirection.Shorter when InfoPanel.CurrentSpriteWavelength != Wavelength.Gamma && InfoPanel.CanCycleSprites:
                case SpectrumDirection.Longer when InfoPanel.CurrentSpriteWavelength != Wavelength.Radio && InfoPanel.CanCycleSprites:
                    canMoveInDirection = true;
                    break;
                default:
                    canMoveInDirection = false;
                    break;
            }
            return canMoveInDirection;
        }

        void IActivatable.Activate()
        {
            if (this is IActivatable activatable && activatable.CanActivate())
            {
                if (direction == SpectrumDirection.Shorter)
                {
                    InfoPanel.CycleSpriteLeft();
                }
                else
                {
                    InfoPanel.CycleSpriteRight();
                }
                spriteWavelength = InfoPanel.CurrentSpriteWavelength;
            }
            SetButtonState();
        }

        private void HandleSpriteWavelengthChanged(SpriteWavelengthChangedEvent e)
        {
            spriteWavelength = e.Wavelength;
            SetButtonState();
        }

        public void SetButtonState()
        {
            switch (spriteWavelength)
            {
                case Wavelength.Gamma when direction == SpectrumDirection.Shorter:
                case Wavelength.Radio when direction == SpectrumDirection.Longer:
                    SetVisibleAndInteractableState(false);
                    break;
                default:
                    SetVisibleAndInteractableState(true);
                    break;
            }
        }

        public void SetVisibleAndInteractableState(bool visible)
        {
            _image.color = visible ? activatableColour : inactiveColour;           
            _collider.enabled = visible;
        }
        
        private void SetAndCheckReferences()
        {
            _image = GetComponent<Image>();
            Assert.IsNotNull(_image, $"{gameObject.name} has no Image component.");

            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_collider, $"<b>[{GetType().Name}]</b> has no Collider component.");

            if (InfoPanel == null)
            {
                InfoPanel = GetComponentInParent<InfoPanelBase>();
            }
            Assert.IsNotNull(InfoPanel, $"<b>[{GetType().Name}]</b> Info panel is not assigned and cannot be found in parent(s).");
        }
    }
}