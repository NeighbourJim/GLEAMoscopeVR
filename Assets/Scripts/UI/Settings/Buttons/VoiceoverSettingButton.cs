using System.Linq;
using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Interaction;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.Settings
{
    public class VoiceoverSettingButton : MonoBehaviour, IActivatable
    {
        [Header("Settings")]
        [SerializeField] private VoiceoverSetting voiceoverSetting = VoiceoverSetting.Female;

        [Space]
        public Image SelectedImage;
        public VoiceoverSettingButton AlternateButton = null;
        public EnableVoiceoverWTButton EnableVoiceoverButton = null;
        
        [Header("IActivatable")]
        [SerializeField] private float activationTime = 2f;
        [SerializeField] private bool isActivated = false;
        [SerializeField] private bool canActivate = true;

        public VoiceoverSetting ButtonSetting => voiceoverSetting;

        float IActivatable.ActivationTime => activationTime;
        bool IActivatable.IsActivated => isActivated;
        
        #region Unity Methods
        void Start()
        {
            SetAndCheckReferences();

            SetDisplayValues();
        }

        void OnEnable()
        {
            EventManager.Instance.AddListener<VoiceoverSettingChangedEvent>(HandleVoiceoverSettingsChanged);
        }

        void OnDisable()
        {
            EventManager.Instance.RemoveListener<VoiceoverSettingChangedEvent>(HandleVoiceoverSettingsChanged);
        }
        #endregion

        private void HandleVoiceoverSettingsChanged(VoiceoverSettingChangedEvent e)
        {
            SetDisplayValues();
        }
        
        private void SetDisplayValues()
        {
            if (SettingsManager.Instance.CurrentVoiceoverSetting == voiceoverSetting)
            {
                SetActiveState();
            }
            else
            {
                Deactivate();
            }
        }

        public void SetActiveState()
        {
            SelectedImage.enabled = true;
            canActivate = false;
        }

        bool IActivatable.CanActivate()
        {
            return !isActivated && SettingsManager.Instance.CurrentVoiceoverSetting != voiceoverSetting;
        }

        void IActivatable.Activate()
        {
            isActivated = true;
            SelectedImage.enabled = true;
            AlternateButton.Deactivate();
            EnableVoiceoverButton.SetCheckedState();
            SettingsManager.Instance.SetVoiceSetting(voiceoverSetting);
        }

        public void Deactivate()
        {
            isActivated = false;
            SelectedImage.enabled = false;
            canActivate = true;
        }

        private void SetAndCheckReferences()
        {
            Assert.IsNotNull(AlternateButton, $"<b>[{GetType().Name}]</b> Alternate button is not assigned.");
            Assert.IsNotNull(EnableVoiceoverButton, $"<b>[{GetType().Name}]</b> Enable voiceover button button is not assigned.");

            FindObjectsOfType<VoiceoverSettingButton>().ToList().ForEach(b =>
            {
                if (b != this)
                {
                    Assert.IsFalse(b.ButtonSetting == voiceoverSetting,
                        $"<b>[{GetType().Name}]</b> there is more than one voiceover setting button in the scene with same setting.");
                }
            });
        }
    }
}