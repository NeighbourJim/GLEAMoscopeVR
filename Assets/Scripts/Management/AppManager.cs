using System.Linq;
using GLEAMoscopeVR.Cameras;
using GLEAMoscopeVR.Environment;
using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.POIs;
using GLEAMoscopeVR.Settings;
using GLEAMoscopeVR.Spectrum;
using GLEAMoscopeVR.Utility;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR
{
    /// <summary>
    /// Todo: work in progress
    /// </summary>
    public class AppManager : GenericSingleton<AppManager>
    {
        [Header("Intro Sequence")]
        public IntroductionSequenceController IntroductionSequenceController = null;
        
        [Header("Camera")]
        //public CameraBlink CameraBlink;
        public CameraBlinkCanvas CameraBlink;
        
        [Header("Sunset")]
        public SunsetController SunsetController = null;
        [SerializeField] private float sunsetDelay = 3f;

        [Header("Environment")]
        public EnvironmentAssetController EnvironmentAssetController = null;
        public PassiveModeRotator PassiveModeRotator;
        
        [Header("Settings")]
        public SettingsPanelButton WarTableSettingsPanelButton;
        public WarTableSettingsPanel WarTableSettingsPanel;
        
        [Header("Spectrum UI")]
        public SpectrumPip[] SpectrumPips = new SpectrumPip[6];
        public SpectrumButton ShorterButton;
        public SpectrumButton LongerButton;
        //public RadioInfoButton RadioInfoButton;
        
        [Space]
        [SerializeField] private bool introductionSequencePlaying = false;
        [SerializeField] private bool introductionSequenceComplete = false;
        [SerializeField] private bool introductionSequenceSkipped = false;

        [Header("Debugging - Settings")]
        [SerializeField] private ExperienceMode experienceMode = ExperienceMode.Introduction;
        [SerializeField] private ExperienceMode previousExperienceMode;
        //[SerializeField] private VoiceoverSetting voiceSetting = VoiceoverSetting.Female;
        //[SerializeField] private bool blinkInPassiveMode = false;
        //[SerializeField] private bool showSubtitles = true;

        [Header("Debugging - State")]
        [SerializeField] private bool rotatorCanSetTarget = false;
        [SerializeField] private POINode activeNode = null;

        public POINode ActiveNode => activeNode;
        public bool SunsetComplete => SunsetController.SunsetCompleted;
        public bool IntroductionSequencePlaying => IntroductionSequenceController.IsPlaying;
        public bool IntroductionSequenceComplete => IntroductionSequenceController.SequenceComplete;
        public bool RotatorCanSetTarget
        {
            get
            {
                rotatorCanSetTarget = PassiveModeRotator.CanSetRotationTarget();
                return rotatorCanSetTarget;
            }
        }
        
        #region Unity Methods
        protected override void Awake()
        {
            base.Awake();
            SetAndCheckReferences();
        }

        void Start()
        {
            previousExperienceMode = SettingsManager.Instance.CurrentExperienceMode;
            SetAppStateForUserSettings();
        }

        void OnEnable()
        {
            EventManager.Instance.AddListener<TeleportToSceneEvent>(HandleTeleportToScene);

            // Settings
            EventManager.Instance.AddListener<ExperienceModeChangedEvent>(HandleExperienceModeChanged);
            //EventManager.Instance.AddListener<VoiceoverSettingChangedEvent>(HandleVoiceSettingChanged);
            
            // Intro Sequence
            EventManager.Instance.AddListener<IntroductionSequenceStateChangedEvent>(HandleIntroductionSequenceStateChanged);
            EventManager.Instance.AddListener<SunsetStateChangedEvent>(HandleSunsetCompleteEvent);

            // POIs
            EventManager.Instance.AddListener<POINodeActivatedEvent>(UpdateNodeState);

            // Spectrum
            //EventManager.Instance.AddListener<SpectrumStateChangedEvent>(HandleWavelengthChanged);
        }

        void OnDisable()
        {
            EventManager.Instance.RemoveListener<TeleportToSceneEvent>(HandleTeleportToScene);

            // Settings
            EventManager.Instance.RemoveListener<ExperienceModeChangedEvent>(HandleExperienceModeChanged);
            //EventManager.Instance.RemoveListener<VoiceoverSettingChangedEvent>(HandleVoiceSettingChanged);

            // Intro Sequence
            EventManager.Instance.RemoveListener<IntroductionSequenceStateChangedEvent>(HandleIntroductionSequenceStateChanged);
            EventManager.Instance.RemoveListener<SunsetStateChangedEvent>(HandleSunsetCompleteEvent);

            // POIs
            EventManager.Instance.RemoveListener<POINodeActivatedEvent>(UpdateNodeState);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SettingsManager.Instance.SaveSettings();
                Application.Quit();
            }
        }
        #endregion

        private void SetAppStateForUserSettings()
        {
            switch (SettingsManager.Instance.CurrentExperienceMode)
            {
                case ExperienceMode.Exploration:
                case ExperienceMode.Passive:
                    PrepareSceneForSunset();
                    break;
                case ExperienceMode.Introduction:
                    //Todo: set up for replay intro sequence
                default:
                    break;
            }
        }

        private void PrepareSceneForSunset() { }
        
        private void BlinkToExplorationMode()
        {
            PrepareSceneForExplorationOrPassiveMode();
            CameraBlink.EyeClosed.RemoveListener(BlinkToExplorationMode);
        }

        private void PrepareSceneForExplorationOrPassiveMode()
        {
            WarTableSettingsPanelButton.SetVisibleAndInteractableState(true);
            WarTableSettingsPanel.SetVisibleAndInteractableState(false);
            
            ShorterButton.SetVisibleAndInteractableState(true);
            LongerButton.SetVisibleAndInteractableState(true);
        }

        private void BlinkToPassiveMode()
        {
            PrepareSceneForExplorationOrPassiveMode();
            CameraBlink.EyeClosed.RemoveListener(BlinkToPassiveMode);
        }
        
        #region Event Handlers
        private void HandleTeleportToScene(TeleportToSceneEvent e)
        {
            if (SettingsManager.Instance.CurrentExperienceMode == ExperienceMode.Introduction)
            {
                IntroductionSequenceController.TriggerIntroductionSequence();
            }
            else
            {
                SunsetController.StartSunset(sunsetDelay);
            }
        }

        private void HandleIntroductionSequenceStateChanged(IntroductionSequenceStateChangedEvent e)
        {
            switch (e.State)
            {
                case IntroductionSequenceStateChangedEvent.IntroState.Playing:
                    introductionSequencePlaying = true;
                    introductionSequenceComplete = false;
                    introductionSequenceSkipped = false;
                    break;
                case IntroductionSequenceStateChangedEvent.IntroState.Skipped:
                    introductionSequencePlaying = false;
                    introductionSequenceComplete = false;
                    introductionSequenceSkipped = true;
                    SettingsManager.Instance.SetExperienceMode(ExperienceMode.Exploration);
                    break;
                case IntroductionSequenceStateChangedEvent.IntroState.Complete:
                    introductionSequencePlaying = false;
                    introductionSequenceComplete = true;
                    introductionSequenceSkipped = false;
                    //SettingsManager.Instance.SetExperienceMode(ExperienceMode.Exploration);
                    break;
                case IntroductionSequenceStateChangedEvent.IntroState.Inactive:
                default:
                    break;
            }
        }

        private void HandleSunsetCompleteEvent(SunsetStateChangedEvent e)
        {
            var mode = SettingsManager.Instance.CurrentExperienceMode;
            if (e.State == EventState.Completed && mode != ExperienceMode.Introduction)
            {
                ShorterButton.SetVisibleAndInteractableState(true);
                LongerButton.SetVisibleAndInteractableState(true);
                SpectrumPips.ToList().ForEach(p => p.ToggleActivatableStatus(true));
                WarTableSettingsPanelButton.SetVisibleAndInteractableState(true);
            }
        }
        
        private void HandleExperienceModeChanged(ExperienceModeChangedEvent e)
        {
            experienceMode = e.ExperienceMode;
            if(SunsetController.SunsetCompleted)
            {
                switch (experienceMode)
                {
                    case ExperienceMode.Exploration:
                        if (previousExperienceMode == ExperienceMode.Passive)
                        {
                            CameraBlink.EyeClosed.AddListener(BlinkToExplorationMode);
                            CameraBlink.Blink();
                        }
                        break;
                    case ExperienceMode.Passive:
                        CameraBlink.EyeClosed.AddListener(BlinkToPassiveMode);
                        CameraBlink.Blink();
                        break;
                    case ExperienceMode.Introduction:
                        CameraBlink.EyeClosed.AddListener(BlinkAndPlayIntroductionSequence);
                        CameraBlink.Blink();
                        break;
                    default:
                        break;
                }
            }
            previousExperienceMode = experienceMode;
        }

        private void BlinkAndPlayIntroductionSequence()
        {
            SunsetController.ResetSky();
            EnvironmentAssetController.SetEnvironmentAssetRenderState(ExperienceMode.Exploration);
            PassiveModeRotator.ResetStateForIntroSequence();
            IntroductionSequenceController.TriggerIntroductionSequence();
            CameraBlink.EyeClosed.RemoveListener(BlinkAndPlayIntroductionSequence);
        }

        //private void HandleVoiceSettingChanged(VoiceoverSettingChangedEvent e)
        //{
        //    voiceSetting = e.VoiceoverSetting;
        //}
        
        private void UpdateNodeState(POINodeActivatedEvent e)
        {
            if (e.Node == activeNode) return;

            if (activeNode != null)
            {
                activeNode.Deactivate();
            }
            activeNode = e.Node;
        }
        #endregion

        private void SetAndCheckReferences()
        {
            if (IntroductionSequenceController == null)
            {
                IntroductionSequenceController = FindObjectOfType<IntroductionSequenceController>();
            }
            Assert.IsNotNull(IntroductionSequenceController, $"<b>[{GetType().Name}]</b> Intro Sequence Controller is not assigned and cannot be found in the scene.");

            if (CameraBlink == null)
            {
                CameraBlink = Camera.main.GetComponentInChildren<CameraBlinkCanvas>();
            }
            Assert.IsNotNull(CameraBlink, $"<b>[{GetType().Name}]</b> Camera blink is not assigned and cannot be found in the scene.");
            
            if (SunsetController == null)
            {
                SunsetController = FindObjectOfType<SunsetController>();
            }
            Assert.IsNotNull(SunsetController, $"<b>[{GetType().Name}]</b> Sunset Controller is not assigned and cannot be found in the scene.");

            if (EnvironmentAssetController == null)
            {
                EnvironmentAssetController = FindObjectOfType<EnvironmentAssetController>();
            }
            Assert.IsNotNull(EnvironmentAssetController, $"<b>[{GetType().Name}]</b> Environment asset controller is not assigned and cannot be found in the scene.");

            if (PassiveModeRotator == null)
            {
                PassiveModeRotator = FindObjectOfType<PassiveModeRotator>();
            }
            Assert.IsNotNull(PassiveModeRotator, $"<b>[{GetType().Name}]</b> Passive mode rotator is not assigned and cannot be found in the scene.");

            Assert.IsNotNull(WarTableSettingsPanelButton);
            Assert.IsNotNull(WarTableSettingsPanel);
        }
    }
}