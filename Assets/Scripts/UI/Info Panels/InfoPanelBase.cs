using System.Linq;
using GLEAMoscopeVR.Audio;
using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.Settings;
using GLEAMoscopeVR.Spectrum;
using GLEAMoscopeVR.Utility.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.POIs
{
    [RequireComponent(typeof(CanvasGroup))]
    public class InfoPanelBase : MonoBehaviour
    {
        [Header("Image Components")]
        public SpriteRenderer BackgroundSpriteRenderer;
        [Tooltip("Point of Interest Image object.")]
        public Image POIImage;

        [Header("Text Components")]
        [Tooltip("Title text object.")]
        public TextMeshProUGUI TitleText;
        [Tooltip("Distance text object.")]
        public TextMeshProUGUI DistanceText;
        [Tooltip("Sprite Wavelength text object.")]
        public TextMeshProUGUI WavelengthText;
        [Tooltip("Description text object.")]
        public TextMeshProUGUI DescriptionText;
        
        [Header("Buttons")]
        public SwapSpriteButton ShorterButton;
        public SwapSpriteButton LongerButton;
        public MuteVoiceoverButton MuteVoiceoverButton;
        public CloseInfoPanelButton CloseButton;

        protected POIObject currentPOI;
        protected bool canCycleSprites = true;
        protected bool ignoreModeChange = false;


        [HideInInspector]
        public Wavelength CurrentSpriteWavelength { get; internal set; }
        public bool CanCycleSprites => canCycleSprites;

        #region References
        protected CanvasGroup _canvasGroup;
        #endregion

        #region Unity Methods
        protected virtual void Awake()
        {
            SetAndCheckReferences();
        }

        protected virtual void Start()
        {
            SetCanvasGroupAndColliderState(false);
        }

        protected virtual void OnEnable()
        {
            EventManager.Instance.AddListener<POINodeActivatedEvent>(HandlePOINodeActivated);
            EventManager.Instance.AddListener<ExperienceModeChangedEvent>(_ => SetCanvasGroupAndColliderState(false));
            EventManager.Instance.AddListener<VoiceoverSettingChangedEvent>(HandleVoiceoverSettingChanged);
            
        }
        protected virtual void OnDisable()
        {
            EventManager.Instance.RemoveListener<POINodeActivatedEvent>(HandlePOINodeActivated);
            EventManager.Instance.RemoveListener<ExperienceModeChangedEvent>(_ => SetCanvasGroupAndColliderState(false));
            EventManager.Instance.RemoveListener<VoiceoverSettingChangedEvent>(HandleVoiceoverSettingChanged);
        }
        
        #endregion

        #region Event Handlers
        protected virtual void HandlePOINodeActivated(POINodeActivatedEvent e)
        {
            currentPOI = new POIObject(e.Node.Data);
            UpdateDisplay(currentPOI);
        }

        protected virtual void HandleVoiceoverSettingChanged(VoiceoverSettingChangedEvent e)
        {
            MuteVoiceoverButton.SetVisibleAndInteractableState(e.VoiceoverSetting != VoiceoverSetting.None);
        }
        #endregion

        /// <summary>
        /// Update the panel's display of information with a new point of interest.
        /// Takes a POIObject object, and utilises the Name, Distance, Description and Sprite parameters.
        /// </summary>
        /// <param name="poi">The Point of Interest object whose parameters will be displayed on the panel.</param>
        public virtual void UpdateDisplay(POIObject poi)
        {
            currentPOI = poi;
            
            TitleText.text = currentPOI.Name;
            DistanceText.text = $"Distance: {currentPOI.Distance}";
            DescriptionText.text = currentPOI.Description;
            CurrentSpriteWavelength = SpectrumStateController.Instance.CurrentWavelength;
            UpdateSprite(CurrentSpriteWavelength);
            SetCanvasGroupAndColliderState(true);
        }

        #region Sprite Change
        public void CycleSpriteRight()
        {
            if(!canCycleSprites) return;

            if (CurrentSpriteWavelength != Wavelength.Radio)
            {
                CurrentSpriteWavelength++;
            }
            UpdateSprite(CurrentSpriteWavelength);
        }

        public void CycleSpriteLeft()
        {
            if (!canCycleSprites) return;

            if (CurrentSpriteWavelength != Wavelength.Gamma)
            {
                CurrentSpriteWavelength--;
            }
            UpdateSprite(CurrentSpriteWavelength);
        }
        
        protected virtual void UpdateSprite(Wavelength wavelength)
        {
            if (currentPOI == null) return;

            CurrentSpriteWavelength = wavelength;
            POIImage.sprite = currentPOI.Sprites[(int)wavelength];
            WavelengthText.text = wavelength.GetDescription();
            if (SettingsManager.Instance.CurrentExperienceMode == ExperienceMode.Exploration)
            {
                UpdateAlternatePanel(wavelength);
            }
            EventManager.Instance.Raise(new SpriteWavelengthChangedEvent(CurrentSpriteWavelength, $"Sprite wavelength changed to {CurrentSpriteWavelength}"));
        }
        #endregion

        protected virtual void UpdateAlternatePanel(Wavelength wavelength) { }

        #region Visible / Interactable State
        /// <summary>
        /// Sets the CanvasGroup component variables.
        /// </summary>
        /// <param name="visible">Whether the panel should be visible or not.</param>
        public virtual void SetCanvasGroupAndColliderState(bool visible)
        {
            _canvasGroup.alpha = visible ? 1f : 0f;

            BackgroundSpriteRenderer.enabled = visible;

            gameObject.GetComponentsInChildren<Collider>()
                .ToList()
                .ForEach(c => c.enabled = visible);

            ToggleMuteVoiceoverButton(visible && SettingsManager.Instance.CurrentVoiceoverSetting != VoiceoverSetting.None);
        }

        public virtual void ToggleCloseButton(bool interactable)
        {
            CloseButton.SetVisibleAndInteractableState(interactable);
        }

        public virtual void ToggleMuteVoiceoverButton(bool interactable)
        {
            MuteVoiceoverButton.SetVisibleAndInteractableState(interactable);
        }
        #endregion

        protected virtual void SetAndCheckReferences()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            Assert.IsNotNull(_canvasGroup, $"{gameObject.name} does not have a canvas group component.");

            Assert.IsNotNull(BackgroundSpriteRenderer, $"<b>[{GetType().Name} - {gameObject.name}]</b> Background sprite renderer not assigned.");

            Assert.IsNotNull(TitleText, $"<b>[{GetType().Name} - {gameObject.name}]</b> Title text is not assigned.");
            Assert.IsNotNull(DistanceText, $"<b>[{GetType().Name} - {gameObject.name}]</b> Distance text is not assigned.");
            Assert.IsNotNull(WavelengthText, $"<b>[{GetType().Name} - {gameObject.name}]</b> Wavelength text is not assigned.");
            Assert.IsNotNull(DescriptionText, $"<b>[{GetType().Name} - {gameObject.name}]</b> Description text is not assigned.");
            Assert.IsNotNull(POIImage, $"<b>[{GetType().Name} - {gameObject.name}]</b> POI image is not assigned.");
            Assert.IsNotNull(ShorterButton, $"<b>[{GetType().Name} - {gameObject.name}]</b> Shorter button is not assigned.");
            Assert.IsNotNull(LongerButton, $"<b>[{GetType().Name} - {gameObject.name}]</b> Longer button is not assigned.");
        }
    }
}