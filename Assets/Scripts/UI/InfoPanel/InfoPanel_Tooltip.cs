using GLEAMoscopeVR.Spectrum;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// Script that manages the data displayed on the InfoPanel as well as its location in worldspace.
    /// Inherits from <see cref="InfoPanel"/> which defines how the war table info panel behaves.
    /// This script behaves the same but has additional methods. 
    /// </summary>
    public class InfoPanel_Tooltip : InfoPanel
    {
        /// <summary>
        /// Sets the location of the tooltip in worldspace, offset from a transform.
        /// </summary>
        /// <param name="targetTransform">The transform of the object that the info panel will be offset from. This should generally be a Point Of Interest object.</param>
        /// <param name="xOffset">The offset value on the X-axis.</param>
        /// <param name="yOffset">The offset value on the Y-axis.</param>
        /// <param name="zOffset">The offset value on the Z-axis.</param>
        void SetLocation(Transform targetTransform, float xOffset = 0f, float yOffset = 0f, float zOffset = 0f)
        {
            Vector3 targetPos = targetTransform.position;
            gameObject.transform.position =
                new Vector3(targetPos.x + xOffset, targetPos.y + yOffset, targetPos.z + zOffset);
        }

        /// <summary>
        /// Creates a tooltip. Calls the UpdateDisplay() and SetLocation() methods.
        /// </summary>
        /// <param name="point">The Point of Interest object whose parameters will be displayed on the panel.</param>
        /// <param name="targetTransform">The transform of the object that the info panel will be offset from. This should generally be a Point Of Interest object.</param>
        /// <param name="xOffset">The offset value on the X-axis.</param>
        /// <param name="yOffset">The offset value on the Y-axis.</param>
        /// <param name="zOffset">The offset value on the Z-axis.</param>
        public void CreateToolTip(POIObject point, Transform targetTransform, float xOffset, float yOffset, float zOffset)
        {
            #region Assertion
            currentSpriteWavelength = WavelengthStateController.Instance.CurrentWavelength;
            Assert.IsTrue(currentSpriteWavelength == Wavelengths.Visible || currentSpriteWavelength == Wavelengths.Radio, 
                $"{gameObject.name} attempting to display sky POI data when wavelength is not Visible or Radio.");
            #endregion
            
            UpdateDisplay(point);
            SetLocation(targetTransform, xOffset, yOffset, zOffset);
            SetCanvasGroupState(true);
        }
    }
}