using UnityEngine;
using UnityEngine.Assertions;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.UI;
using UnityEngine.UI;

namespace GLEAMoscopeVR.Settings
{
    public class VoiceSelectButton : MonoBehaviour, IActivatable, IHideableUI
    {
        [Header("Settings")]
        [SerializeField] private VoiceoverSetting voiceoverSetting = VoiceoverSetting.Female;

        [Space]
        public VoiceSelectButton AlternateButton = null;
        public EnableVoiceoverButton EnableVoiceoverButton = null;

        [Header("IAactivatable")]
        [SerializeField] private float activationTime = 2f;
        [SerializeField] private bool canActivate = false;

        [Header("Display")]
        [SerializeField] private Color activeColour = new Color(148, 235, 255, 155);
        [SerializeField] private Color inactiveColour = new Color(182, 182, 182, 155);

        #region References
        CanvasGroup _canvasGroup;
        Collider _collider;
        Image _borderImage;
        #endregion

        public VoiceoverSetting ButtonSetting => voiceoverSetting;

        CanvasGroup IHideableUI.CanvasGroup => _canvasGroup;
        Collider IHideableUI.Collider => _collider;
        float IActivatable.ActivationTime => activationTime;
        bool IActivatable.IsActivated => false;
        
        #region Unity Methods
        void Start()
        {
            SetAndCheckReferences();
            SetDisplayValues();
        }
        #endregion

        private void SetDisplayValues()
        {
            if (SettingsManager.Instance.CurrentVoiceoverSetting == voiceoverSetting)
            {
                SetActiveState();
            }
            else
            {
                SetInactiveState();
            }
        }

        public void SetActiveState()
        {
            _borderImage.color = activeColour;
            canActivate = false;
        }

        public void SetInactiveState()
        {
            _borderImage.color = inactiveColour;
            canActivate = true;
        }

        bool IActivatable.CanActivate() => canActivate;

        void IActivatable.Activate()
        {
            SettingsManager.Instance.SetVoiceSetting(voiceoverSetting);
            SetActiveState();
            AlternateButton.SetInactiveState();
            EnableVoiceoverButton.SetCheckedState();
        }

        public void SetVisibleAndInteractableState(bool visible)
        {
            canActivate = visible;
            _canvasGroup.alpha = visible ? 1 : 0;
            _collider.enabled = visible;
        }

        private void SetAndCheckReferences()
        {
            _canvasGroup = GetComponentInParent<CanvasGroup>();
            Assert.IsNotNull(_canvasGroup, $"<b>[{GetType().Name}]</b> has no Canvas Group component in parent.");

            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_collider, $"<b>[{GetType().Name}]</b> has no collider component.");

            _borderImage = GetComponent<Image>();
            Assert.IsNotNull(_borderImage, $"<b>[VoiceSelectButton]</b> has no Image component (How?).");

            Assert.IsNotNull(AlternateButton, $"<b>[{GetType().Name}]</b> alternate button not assigned.");
            Assert.IsNotNull(EnableVoiceoverButton, $"<b>[{GetType().Name}]</b> enable voiceover button not assigned.");
        }
    }
}