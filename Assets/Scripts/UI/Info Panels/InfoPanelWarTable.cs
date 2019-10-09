using GLEAMoscopeVR.Audio;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.Settings;
using GLEAMoscopeVR.Spectrum;
using GLEAMoscopeVR.Utility.Extensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.POIs
{
    public class InfoPanelWarTable : InfoPanelBase
    {
        [Header("Alternate Panel")]
        public InfoPanelSky SkyPanel;

        [Header("War Table Components")]
        public MaximiseButton MaximiseButton;
        public WarTableSettingsPanel SettingsPanel;

        public override void UpdateDisplay(POIObject poi)
        {
            base.UpdateDisplay(poi);
            UpdatePanelState();
        }

        private void UpdatePanelState()
        {
            if (SettingsPanel.IsActive)
            {
                SettingsPanel.SetVisibleAndInteractableState(false);
            }

            if (MaximiseButton != null && MaximiseButton.IsActivated)
            {
                MaximiseButton.SetVisibleAndInteractableState(false);
            }

            ToggleMuteVoiceoverButton(SettingsManager.Instance.CurrentVoiceoverSetting != VoiceoverSetting.None);
            ToggleCloseButton(true);
        }

        protected override void UpdateAlternatePanel(Wavelength wavelength)
        {
            SkyPanel.CurrentSpriteWavelength = wavelength;
            SkyPanel.POIImage.sprite = currentPOI.Sprites[(int) wavelength];
            SkyPanel.WavelengthText.text = wavelength.GetDescription();
        }

        protected override void SetAndCheckReferences()
        {
            base.SetAndCheckReferences();

            if (MuteVoiceoverButton == null)
            {
                MuteVoiceoverButton = GetComponentInChildren<MuteVoiceoverButton>();
            }
            Assert.IsNotNull(MuteVoiceoverButton, $"<b>[{GetType().Name} - {gameObject.name}]</b> Mute voiceover button is not assigned and not found in children.");

            if (CloseButton == null)
            {
                CloseButton = GetComponentInChildren<CloseInfoPanelButton>();
            }
            Assert.IsNotNull(CloseButton, $"<b>[{GetType().Name} - {gameObject.name}]</b> Close panel button not found in children.");

            if (SkyPanel == null)
            {
                SkyPanel = FindObjectOfType<InfoPanelSky>();
            }
            Assert.IsNotNull(SkyPanel, $"<b>[{GetType().Name}] Sky panel not assigned and cannot be found in scene.");

            if (MaximiseButton == null)
            {
                MaximiseButton = FindObjectOfType<MaximiseButton>();
            }
            Assert.IsNotNull(MaximiseButton, $"<b>[{GetType().Name}] Maximise button not assigned and cannot be found in scene.");

            if(SettingsPanel == null)
            {
                SettingsPanel = FindObjectOfType<WarTableSettingsPanel>();
            }
            Assert.IsNotNull(SettingsPanel, $"<b>[{GetType().Name}] War table settings panel not assigned and cannot be found in scene.");
        }
    }
}