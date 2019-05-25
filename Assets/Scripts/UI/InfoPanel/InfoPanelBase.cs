using GLEAMoscopeVR.Settings;
using GLEAMoscopeVR.Spectrum;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// MM: Updated 21/05/19 Default sprite set to Radio. (will only ever start at Visible or Radio)
    /// Todo: Add method for swapping out sprites when buttons are activated.
    /// Base class for <see cref="POIObject"/> data display panels.
    /// /// When a POI is activated, the sprite will always start at either Visible or Radio.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class InfoPanelBase : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField]
        [Tooltip("Title text object.")]
        protected TextMeshProUGUI TitleText;
        [SerializeField]
        [Tooltip("Distance text object.")]
        protected TextMeshProUGUI DistanceText;
        [SerializeField]
        [Tooltip("Description text object.")]
        protected TextMeshProUGUI DescriptionText;
        [SerializeField]
        [Tooltip("Point of Interest Image object.")]
        protected Image PointImage;
        [Tooltip("Current Point of Interest")]
        POIObject currentPOI = null;
        [Tooltip("Current POI Sprite Index")]
        Wavelengths currentSpriteWavelength;

        #region References
        /// <summary>
        /// Used to toggle display panel.
        /// </summary>
        protected CanvasGroup _canvasGroup;
        protected ExperienceModeController _modeController;
        #endregion

        #region Unity Methods
        protected virtual void Awake()
        {
            GetComponentReferences();
            SetCanvasGroupState(false);
        }
        #endregion

        /// <summary>
        /// Update the panel's display of information with a new point of interest.
        /// Takes a POIObject object, and utilises the Name, Distance, Description and Sprite parameters.
        /// </summary>
        /// <param name="point">The Point of Interest object whose parameters will be displayed on the panel.</param>
        public virtual void UpdateDisplay(POIObject poi)
        {
            currentPOI = poi;
            currentSpriteWavelength = Wavelengths.Radio;

            TitleText.text = currentPOI.Name;
            DistanceText.text = $"Distance: {currentPOI.Distance}";
            DescriptionText.text = currentPOI.Description;
            UpdateSprite(currentSpriteWavelength);

            SetCanvasGroupState(true);
        }

        public virtual void CycleSpriteRight()
        {
            if(currentSpriteWavelength != Wavelengths.Radio)
            {
                currentSpriteWavelength++;
            }
            else
            {
                currentSpriteWavelength = Wavelengths.Gamma;
            }
            UpdateSprite(currentSpriteWavelength);
        }

        public virtual void CycleSpriteLeft()
        {
            if (currentSpriteWavelength != Wavelengths.Gamma)
            {
                currentSpriteWavelength++;
            }
            else
            {
                currentSpriteWavelength = Wavelengths.Radio;
            }
            UpdateSprite(currentSpriteWavelength);
        }

        private void UpdateSprite(Wavelengths wavelength)
        {
            if (currentPOI != null)
            {
                PointImage.sprite = currentPOI.Sprites[(int)wavelength];
            }
            else
            {
                Debug.LogError("The point of interest is not set for this info panel! This should never happen.");
            }
        }

        /// <summary>
        /// Sets the CanvasGroup component variables.
        /// </summary>
        /// <param name="visible">Whether the panel should be visible or not.</param>
        /// <param name="interactable">Determines if this component will accept input. When it is set to false interaction is disabled.</param>
        /// <param name="blocksRaycast">Determines whether the panel should act as a collider for raycasts.</param>
        public virtual void SetCanvasGroupState(bool visible, bool interactable = false, bool blocksRaycast = false)
        {
            _canvasGroup.alpha = visible ? 1f : 0f;
            _canvasGroup.interactable = interactable;
            _canvasGroup.blocksRaycasts = blocksRaycast;
        }

        protected virtual void GetComponentReferences()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            Assert.IsNotNull(_canvasGroup, $"{gameObject.name} does not have a canvas group attached.");
        }
    }
}