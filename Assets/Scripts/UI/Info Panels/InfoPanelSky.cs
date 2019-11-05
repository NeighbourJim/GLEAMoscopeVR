using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Settings;
using GLEAMoscopeVR.Spectrum;
using GLEAMoscopeVR.Utility.Extensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// Script that manages the data displayed on the InfoPanel as well as its location in worldspace.
    /// Inherits from <see cref="InfoPanelBase"/> which defines how the war table info panel behaves.
    /// This script behaves the same but has additional methods. 
    /// </summary>
    public class InfoPanelSky : InfoPanelBase
    {
        [Header("Alternate Panel")]
        public InfoPanelWarTable WarTablePanel;

        protected override void HandlePOINodeActivated(POINodeActivatedEvent e)
        {
            if (SettingsManager.Instance.CurrentExperienceMode == ExperienceMode.Passive) return;//allows for forcing first sky POI activation in intro sequence.
            
            CreateToolTip(new POIObject(e.Node.Data), e.Node.transform, e.Node.Data.XOffset, e.Node.Data.YOffset, e.Node.Data.ZOffset);
        }
        
        /// <summary>
        /// Creates a tooltip. Calls the UpdateDisplay() and SetLocation() methods.
        /// </summary>
        /// <param name="poi">The Point of Interest object whose parameters will be displayed on the panel.</param>
        /// <param name="targetTransform">The transform of the object that the info panel will be offset from. This should generally be a Point Of Interest object.</param>
        /// <param name="xOffset">The offset value on the X-axis.</param>
        /// <param name="yOffset">The offset value on the Y-axis.</param>
        /// <param name="zOffset">The offset value on the Z-axis.</param>
        public void CreateToolTip(POIObject poi, Transform targetTransform, float xOffset, float yOffset, float zOffset)
        {
            #region Assertion
            CurrentSpriteWavelength = SpectrumStateController.Instance.CurrentWavelength;
            Assert.IsTrue(CurrentSpriteWavelength == Wavelength.Visible || CurrentSpriteWavelength == Wavelength.Radio, 
                $"{gameObject.name} attempting to display sky POI data when wavelength is not Visible or Radio.");
            #endregion

            //SetCanvasGroupAndColliderState(false);
            SetLocation(targetTransform, xOffset, yOffset, zOffset);
            UpdateDisplay(poi);
        }

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

        protected override void UpdateAlternatePanel(Wavelength wavelength)
        {
            WarTablePanel.CurrentSpriteWavelength = wavelength;
            WarTablePanel.POIImage.sprite = currentPOI.Sprites[(int)wavelength];
            switch (SettingsManager.Instance.CurrentLanguageSetting)
            {
                case LanguageSetting.English:
                    WarTablePanel.WavelengthText.text = englishSpectrum[(int)wavelength];
                    break;
                case LanguageSetting.Italian:
                    WarTablePanel.WavelengthText.text = italianSpectrum[(int)wavelength];
                    break;
            }
        }

        protected override void SetAndCheckReferences()
        {
            base.SetAndCheckReferences();

            if (WarTablePanel == null)
            {
                WarTablePanel = FindObjectOfType<InfoPanelWarTable>();
            }
            Assert.IsNotNull(WarTablePanel, $"<b>[{GetType().Name}] War table panel not assigned and cannot be found in scene.");
        }
    }
}