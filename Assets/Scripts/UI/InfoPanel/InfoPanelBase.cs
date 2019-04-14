using GLEAMoscopeVR.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// Base class for <see cref="PointOfInterest"/> data display panels.
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
        /// Takes a PointOfInterest object, and utilises the Name, Distance, Description and Sprite parameters.
        /// </summary>
        /// <param name="point">The Point of Interest object whose parameters will be displayed on the panel.</param>
        public virtual void UpdateDisplay(PointOfInterest poi)
        {
            TitleText.text = poi.Name;
            DistanceText.text = $"Distance: {poi.Distance}";
            DescriptionText.text = poi.Description;
            PointImage.sprite = poi.Sprite;

            SetCanvasGroupState(true);
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