using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.POIs;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.Settings
{
    public class SettingsPanelButton : MonoBehaviour, IActivatable
    {
        [SerializeField]
        private float activationTime = 2f;
        public float ActivationTime => activationTime;

        [SerializeField]
        private bool isActivated = false;
        public bool IsActivated => isActivated;

        public InfoPanelWarTable WarTablePanel;
        public WarTableSettingsPanel SettingsPanel;

        TextMeshProUGUI _text;
        Image _image;
        Collider _collider;

        void Awake()
        {
            SetAndCheckReferences();
            SetVisibleAndInteractableState(false);
        }

        void OnEnable()
        {
            EventManager.Instance.AddListener<IntroductionSequenceStateChangedEvent>(HandleIntroductionSequenceStateChanged);
            EventManager.Instance.AddListener<ExperienceModeChangedEvent>(HandleExperienceModeChanged);
        }

        void OnDisable()
        {
            EventManager.Instance.RemoveListener<IntroductionSequenceStateChangedEvent>(HandleIntroductionSequenceStateChanged);
            EventManager.Instance.RemoveListener<ExperienceModeChangedEvent>(HandleExperienceModeChanged);
        }

        public bool CanActivate()
        {
            return !isActivated;
        }

        public void Activate()
        {
            WarTablePanel.SetCanvasGroupAndColliderState(false);
            SettingsPanel.SetVisibleAndInteractableState(true);
        }

        public void Deactivate(){}
        
        private void HandleExperienceModeChanged(ExperienceModeChangedEvent e)
        {
            if (AppManager.Instance.IntroductionSequencePlaying)
            {
                return;
            }

            switch (e.ExperienceMode)
            {
                case ExperienceMode.Introduction:
                    SetVisibleAndInteractableState(false);
                    break;
                case ExperienceMode.Exploration:
                case ExperienceMode.Passive:
                    SetVisibleAndInteractableState(true);
                    break;
            }
        }

        private void HandleIntroductionSequenceStateChanged(IntroductionSequenceStateChangedEvent e)
        {
            SetVisibleAndInteractableState(e.State != IntroductionSequenceStateChangedEvent.IntroState.Playing);
        }

        public void SetVisibleAndInteractableState(bool visible)
        {
            _text.enabled = visible;
            _image.enabled = visible;
            _collider.enabled = visible;
        }
        
        private void SetAndCheckReferences()
        {
            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_collider, $"<b>[{GetType().Name}]</b> has no Collider component.");

            _image = GetComponent<Image>();
            Assert.IsNotNull(_image, $"<b>[{GetType().Name}]</b> has no image component.");

            _text = GetComponentInChildren<TextMeshProUGUI>();
            Assert.IsNotNull(_text, $"<b>[{GetType().Name}]</b> has no Text component in children.");
        }
    }
}