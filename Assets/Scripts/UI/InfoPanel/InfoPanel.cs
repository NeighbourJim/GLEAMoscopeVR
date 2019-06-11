using GLEAMoscopeVR.Settings;
using GLEAMoscopeVR.Spectrum;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// Base class for <see cref="POIObject"/> data display panels.
    /// When a POI is activated, the sprite will always start at either Visible or Radio.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class InfoPanel : MonoBehaviour
    {
        private const string antennaPOIName = "Antenna";

        [Header("UI Components")]
        [SerializeField]
        [Tooltip("Title text object.")]
        protected TextMeshProUGUI titleText;
        [SerializeField]
        [Tooltip("Distance text object.")]
        protected TextMeshProUGUI distanceText;
        [SerializeField]
        [Tooltip("Sprite Wavelength text object.")]
        protected TextMeshProUGUI wavelengthText;
        [SerializeField]
        [Tooltip("Description text object.")]
        protected TextMeshProUGUI descriptionText;
        [SerializeField]
        [Tooltip("Point of Interest Image object.")]
        protected Image poiImage;
        protected POIObject currentPOI;

        
        [Tooltip("Current POI Sprite Index")]
        protected Wavelengths currentSpriteWavelength;
        protected bool canCycleSprites = false;
        public bool CanCycleSprites => canCycleSprites;

        #region References
        /// <summary> Used to toggle panel visibility. </summary>
        protected CanvasGroup _canvasGroup;
        #endregion

        #region Unity Methods
        protected virtual void Awake()
        {
            SetAndCheckReferences();
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
            canCycleSprites = poi.Data.Name != antennaPOIName;
            
            titleText.text = currentPOI.Name;
            distanceText.text = $"Distance: {currentPOI.Distance}";
            descriptionText.text = currentPOI.Description;

            UpdateSprite(WavelengthStateController.Instance.CurrentWavelength);//currentSpriteWavelength);            
            SetCanvasGroupState(true);
        }

        public void CycleSpriteRight()
        {
            if(canCycleSprites)
            {
                if (currentSpriteWavelength != Wavelengths.Radio)
                {
                    currentSpriteWavelength++;
                }
                else
                {
                    currentSpriteWavelength = Wavelengths.Gamma;
                }

                UpdateSprite(currentSpriteWavelength);
            }
        }

        public void CycleSpriteLeft()
        {
            if(canCycleSprites)
            {
                if (currentSpriteWavelength != Wavelengths.Gamma)
                {
                    currentSpriteWavelength--;
                }
                else
                {
                    currentSpriteWavelength = Wavelengths.Radio;
                }

                UpdateSprite(currentSpriteWavelength);
            }
        }
        

        protected void UpdateSprite(Wavelengths wavelength)
        {
            if (currentPOI != null)
            {
                poiImage.sprite = currentPOI.Name == antennaPOIName ? currentPOI.Sprites[0] : currentPOI.Sprites[(int) wavelength];
                switch(wavelength)
                {
                    case Wavelengths.XRay:
                        wavelengthText.text = "X-Ray";
                        break;
                    case Wavelengths.Infrared:
                        wavelengthText.text = "Far-Infrared";
                        break;
                    default:
                        wavelengthText.text = wavelength.ToString();
                        break;
                }
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

        protected virtual void SetAndCheckReferences()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            Assert.IsNotNull(_canvasGroup, $"{gameObject.name} does not have a canvas group component.");
        }
    }
}