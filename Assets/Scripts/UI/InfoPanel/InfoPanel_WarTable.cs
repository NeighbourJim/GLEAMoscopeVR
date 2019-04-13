using MM.GLEAMoscopeVR.POIs;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace TBD
{
    /// <summary>
    /// Script that manages the data displayed on the InfoPanel above the war table.
    /// The code is almost the same as the <see cref="InfoPanel_Manager"/> script.
    /// Will allow for additional data to be displayed on this panel in future (if required).
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class InfoPanel_WarTable : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Title text object.")]
        private TextMeshProUGUI TitleText;
        [SerializeField]
        [Tooltip("Distance text object.")]
        private TextMeshProUGUI DistanceText;
        [SerializeField]
        [Tooltip("Description text object.")]
        private TextMeshProUGUI DescriptionText;
        [SerializeField]
        [Tooltip("Point of Interest Image object.")]
        private Image Image;
        
        /// <summary>
        /// Used to toggle display panel.
        /// </summary>
        CanvasGroup _canvasGroup;

        void Awake()
        {
            GetComponentReferences();
        }

        /// <summary>
        /// Update the panel's display of information with a new point of interest.
        /// Takes a PointOfInterest object, and utilises the Name, Distance, Description and Sprite parameters.
        /// </summary>
        /// <param name="point">The Point of Interest object whose parameters will be displayed on the panel.</param>
        void UpdateDisplay(PointOfInterest poi)
        {
            TitleText.text = poi.Name;
            DistanceText.text = $"Distance: {poi.Distance}";
            DescriptionText.text = poi.Description;
            Image.sprite = poi.Sprite;
            ToggleCanvasGroupAlpha(true);
        }

        /// <summary>
        /// Sets the alpha value on the Canvas Group component.
        /// </summary>
        /// <param name="visible">Whether the panel should be visible or not.</param>
        void ToggleCanvasGroupAlpha(bool visible)
        {
            _canvasGroup.alpha = visible ? 1f : 0f;
        }

        private void GetComponentReferences()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            Assert.IsNotNull(_canvasGroup, $"[InfoPanel_Manager] {gameObject.name} does not have a canvas group attached.");
            ToggleCanvasGroupAlpha(false);
        }
    }
}