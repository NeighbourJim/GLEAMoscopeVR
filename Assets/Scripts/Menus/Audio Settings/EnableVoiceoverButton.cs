using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.Settings
{
    public class EnableVoiceoverButton : MonoBehaviour, IActivatable, IHideableUI
    {
        public Image CheckedImage;

        [Space]
        public VoiceSelectButton FemaleButton;
        public VoiceSelectButton MaleButton;

        [Space] [SerializeField] private bool isChecked = true;

        [Header("IActivatable")]
        [SerializeField] private float activationTime = 2f;
        [SerializeField] private bool canActivate = true;

        #region References
        CanvasGroup _canvasGroup;
        Collider _collider;
        #endregion

        CanvasGroup IHideableUI.CanvasGroup => _canvasGroup;
        Collider IHideableUI.Collider => _collider;

        float IActivatable.ActivationTime => activationTime;
        bool IActivatable.IsActivated => true;

        #region Unity Methods
        void Awake()
        {
            SetAndCheckReferences();
        }

        void Start()
        {
            SetCheckedState();
        }
        #endregion

        public void SetCheckedState()
        {
            var voiceSetting = SettingsManager.Instance.CurrentVoiceoverSetting;
            isChecked = voiceSetting != VoiceoverSetting.None;
            CheckedImage.enabled = voiceSetting != VoiceoverSetting.None;
        }

        bool IActivatable.CanActivate()
        {
            return canActivate;
        }

        void IActivatable.Activate()
        {
            if (isChecked)
            {
                FemaleButton.SetInactiveState();
                MaleButton.SetInactiveState();
                SettingsManager.Instance.SetVoiceSetting(VoiceoverSetting.None);
            }
            else
            {
                FemaleButton.SetActiveState();
                MaleButton.SetInactiveState();
                SettingsManager.Instance.SetVoiceSetting(VoiceoverSetting.Female);
            }

            SetCheckedState();
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

            Assert.IsNotNull(CheckedImage, $"<b>[{GetType().Name}]</b> Checked image is not assigned.");

            Assert.IsNotNull(FemaleButton, $"<b>[{GetType().Name}]</b> Female button not assigned.");
            Assert.IsTrue(FemaleButton.ButtonSetting == VoiceoverSetting.Female, $"<b>[{GetType().Name}]</b> Female button is assigned but the settings on the button is not Female.");
            Assert.IsNotNull(MaleButton, $"<b>[{GetType().Name}]</b> Male button not assigned.");
            Assert.IsTrue(MaleButton.ButtonSetting == VoiceoverSetting.Male, $"<b>[{GetType().Name}]</b> Male button is assigned but the settings on the button is not Male.");

        }
    }
}