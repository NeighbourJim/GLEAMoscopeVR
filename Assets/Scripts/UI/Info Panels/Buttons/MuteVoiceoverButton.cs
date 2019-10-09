using System.Linq;
using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.POIs;
using GLEAMoscopeVR.Settings;
using GLEAMoscopeVR.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.Audio
{
    public class MuteVoiceoverButton : MonoBehaviour, IActivatable, IHideableUI
    {
        [SerializeField] private bool mute = false;

        [Header("Sprites")]
        public Image BorderImage;
        public Image IconImage;
        public Sprite MuteSprite;

        [Space] public MuteVoiceoverButton AlternateMuteButton;

        [Header("IActivatable")]
        [SerializeField] private float activationTime = 1f;
        [SerializeField] private bool isActivated = false;
        [SerializeField] private bool voiceoverEnabled = true;
        
        [Header("Colours")]
        [SerializeField] private Color activatableColour;
        [SerializeField] private Color inactiveColour = new Color(68, 68, 68);

        POINode previousNode = null;
        POINode currentNode = null;

        CanvasGroup _canvasGroup;
        Collider _collider;
        
        float IActivatable.ActivationTime => activationTime;
        bool IActivatable.IsActivated => mute;

        CanvasGroup IHideableUI.CanvasGroup => _canvasGroup;
        Collider IHideableUI.Collider => _collider;

        void Awake()
        {
            SetAndCheckReferences();
        }

        void Start()
        {
            voiceoverEnabled = SettingsManager.Instance.CurrentVoiceoverSetting != VoiceoverSetting.None;
            SetVisibleAndInteractableState(voiceoverEnabled);
        }

        void OnEnable()
        {
            //EventManager.Instance.AddListener<POINodeActivatedEvent>(HandlePOIActivated);
            EventManager.Instance.AddListener<VoiceoverSettingChangedEvent>(HandleVoiceoverSettingChanged);
        }

        void OnDisable()
        {
            //EventManager.Instance.RemoveListener<POINodeActivatedEvent>(HandlePOIActivated);
            EventManager.Instance.RemoveListener<VoiceoverSettingChangedEvent>(HandleVoiceoverSettingChanged);
        }
        
        public bool CanActivate()
        {
            return voiceoverEnabled && VoiceoverController.Instance.IsPlaying;
        }

        void IActivatable.Activate()
        {
            if (CanActivate())
            {
                VoiceoverController.Instance.StopAudioClip();
                SetVisibleAndInteractableState(false);
                AlternateMuteButton.SetVisibleAndInteractableState(false);
            }
        }
        
        private void HandleVoiceoverSettingChanged(VoiceoverSettingChangedEvent e)
        {
            voiceoverEnabled = e.VoiceoverSetting != VoiceoverSetting.None;
            SetVisibleAndInteractableState(voiceoverEnabled);
        }
        
        public void SetVisibleAndInteractableState(bool visible)
        {
            GetComponent<Collider>().enabled = visible;
            BorderImage.color = visible ? activatableColour : inactiveColour;
            IconImage.color = visible ? activatableColour : inactiveColour;
        }

        private void SetAndCheckReferences()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            Assert.IsNotNull(_canvasGroup, $"<b>[{GetType().Name}]</b> has no Canvas Group component.");

            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_canvasGroup, $"<b>[{GetType().Name}]</b> has no Collider component.");

            Assert.IsNotNull(BorderImage, $"<b>[{GetType().Name}]</b> Border image is not assigned.");
            Assert.IsNotNull(IconImage, $"<b>[{GetType().Name}]</b> Icon image is not assigned.");
            Assert.IsNotNull(MuteSprite, $"<b>[{GetType().Name}]</b> Mute sprite is not assigned.");
            
            Assert.IsNotNull(AlternateMuteButton, $"<b>[{GetType().Name}]</b> Alternate mute button is not assigned.");
            Assert.IsFalse(AlternateMuteButton == this, $"<b>[{GetType().Name} - {gameObject.name}]</b> Alternate mute button is set to this mute button.");
        }
    }
}