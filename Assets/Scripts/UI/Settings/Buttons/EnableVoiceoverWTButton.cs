using GLEAMoscopeVR.Audio;
using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.Settings
{
    public class EnableVoiceoverWTButton : MonoBehaviour, IActivatable, IHideableUI
    {
        [SerializeField] private bool isChecked = true;

        public Image SelectedImage;

        public VoiceoverSettingButton FemaleButton;
        public VoiceoverSettingButton MaleButton;

        [Header("IActivatable")]
        [SerializeField] private float activationTime = 2f;
        [SerializeField] private bool canActivate = true;

        #region References
        CanvasGroup _canvasGroup;
        Collider _collider;
        #endregion

        float IActivatable.ActivationTime => activationTime;
        bool IActivatable.IsActivated => true;
        CanvasGroup IHideableUI.CanvasGroup => _canvasGroup;
        Collider IHideableUI.Collider => _collider;

        void Start()
        {
            SetAndCheckReferences();
            SetCheckedState();
        }

        void OnEnable()
        {
            EventManager.Instance.AddListener<VoiceoverSettingChangedEvent>(HandleVoiceoverSettingsChanged);
        }

        void OnDisable()
        {
            EventManager.Instance.RemoveListener<VoiceoverSettingChangedEvent>(HandleVoiceoverSettingsChanged);
        }

        private void HandleVoiceoverSettingsChanged(VoiceoverSettingChangedEvent e)
        {
            SetCheckedState();
        }

        bool IActivatable.CanActivate()
        {
            return canActivate;
        }

        void IActivatable.Activate()
        {
            SettingsManager.Instance.SetVoiceSetting(isChecked ? VoiceoverSetting.None : VoiceoverSetting.Female);
            if (SettingsManager.Instance.CurrentVoiceoverSetting == VoiceoverSetting.None)
            {
                VoiceoverController.Instance.StopAudioClip(true);
            }
            SetCheckedState();
        }

        public void SetCheckedState()
        {
            isChecked = SettingsManager.Instance.CurrentVoiceoverSetting != VoiceoverSetting.None;
            SelectedImage.enabled = isChecked;
        }
        
        public void SetVisibleAndInteractableState(bool visible)
        {
            canActivate = visible;
            _collider.enabled = visible;
        }

        private void SetAndCheckReferences()
        {
            _canvasGroup = GetComponentInParent<CanvasGroup>();
            Assert.IsNotNull(_canvasGroup, $"<b>[{GetType().Name}]</b> has no Canvas Group component in parent.");

            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_collider, $"<b>[{GetType().Name}]</b> has no collider component.");

            Assert.IsNotNull(SelectedImage, $"<b>[{GetType().Name}]</b> Selected image is not assigned.");

            Assert.IsNotNull(FemaleButton, $"<b>[{GetType().Name}]</b> Female button not assigned.");
            Assert.IsTrue(FemaleButton.ButtonSetting == VoiceoverSetting.Female, $"<b>[{GetType().Name}]</b> Female button is assigned but the settings on the button is not Female.");
            Assert.IsNotNull(MaleButton, $"<b>[{GetType().Name}]</b> Male button not assigned.");
            Assert.IsTrue(MaleButton.ButtonSetting == VoiceoverSetting.Male, $"<b>[{GetType().Name}]</b> Male button is assigned but the settings on the button is not Male.");
        }
    }
}