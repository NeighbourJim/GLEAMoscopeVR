using System.Collections;
using System.Linq;
using GLEAMoscopeVR.Audio;
using GLEAMoscopeVR.Cameras;
using GLEAMoscopeVR.Environment;
using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.POIs;
using GLEAMoscopeVR.Settings;
using GLEAMoscopeVR.Spectrum;
using GLEAMoscopeVR.SubtitleSystem;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Interaction
{
    public class IntroductionSequenceController : MonoBehaviour
    {
        [Header("State (Debugging)")]
        [SerializeField] private bool isPlaying = false;
        [SerializeField] private bool greetingComplete = false;
        [SerializeField] private bool antennaPOIComplete = false;
        [SerializeField] private bool wavelengthChangeComplete = false;
        [SerializeField] private bool radioIntroComplete = false;
        [SerializeField] private bool firstPOIActivated = false;
        [SerializeField] private bool sequenceComplete = false;

        [Header("Greeting")]
        [SerializeField] private float greetingAudioDelay = 2.5f;
        public AudioClip GreetingEngFemale;
        public AudioClip GreetingEngMale;
        public SubtitleData GreetingEngSubtitle;
        [Space]
        public AudioClip GreetingItaFemale;
        public AudioClip GreetingItaMale;
        public SubtitleData GreetingItaSubtitle;

        [Header("Antenna POI")]
        public AntennaPOI AntennaNode;
        public AudioClip AntennaEngFemale;
        public AudioClip AntennaEngMale;
        public SubtitleData AntennaEngSubtitle;
        public InfoPanelAntenna AntennaInfoPanel;
        [Space]
        public AudioClip AntennaItaFemale;
        public AudioClip AntennaItaMale;
        public SubtitleData AntennaItaSubtitle;

        [Header("Wavelength Prompt")]
        [SerializeField] private float changeWavelengthAudioDelay = 5f;
        public AudioClip WavelengthEngFemale;
        public AudioClip WavelengthEngMale;
        public SubtitleData WavelengthEngSubtitle;
        public RadioWavelengthPrompt RadioPrompt;
        [Space]
        public AudioClip WavelengthItaFemale;
        public AudioClip WavelengthItaMale;
        public SubtitleData WavelengthItaSubtitle;

        [Header("Radio Intro")]
        [SerializeField] private float radioIntroDelay = 1f;
        public AudioClip RadioIntroEngFemale;
        public AudioClip RadioIntroEngMale;
        public SubtitleData RadioIntroEngSubtitle;
        //public RadioInfoButton RadioInfoButton;
        [Space]
        public AudioClip RadioIntroItaFemale;
        public AudioClip RadioIntroItaMale;
        public SubtitleData RadioIntroItaSubtitle;

        [Header("Sky POI Prompt")]
        public GameObject SkyPOIPrompt;
        
        [Header("System References")]
        public SubtitleDisplayer SubtitleDisplayer;
        public SunsetController SunsetController;

        [Header("UI")]
        public SpectrumCanvas SpectrumCanvas;
        public SkipIntroButton SkipIntroButton;

        [Space] public CameraBlinkCanvas CameraBlink;
        
        Coroutine introductionRoutine;

        AudioSource _audioSource;

        public bool IsPlaying => isPlaying;
        public bool SequenceComplete => sequenceComplete;

        void Start()
        {
            SetAndCheckReferences();
            ResetStateVariables();
        }
        
        public void TriggerIntroductionSequence()
        {
            if (SettingsManager.Instance.CurrentExperienceMode == ExperienceMode.Introduction)
            {
                ResetStateVariables();
                SubtitleDisplayer.StopSubtitles();
                VoiceoverController.Instance.StopAudioClip(true);
                introductionRoutine = StartCoroutine(PlayIntroductionSequence());
            }
        }
        
        IEnumerator PlayIntroductionSequence()
        {
            isPlaying = true;
            EventManager.Instance.Raise(new IntroductionSequenceStateChangedEvent(IntroductionSequenceStateChangedEvent.IntroState.Playing, $"Introduction sequence started."));

            var playVoiceover = SettingsManager.Instance.CurrentVoiceoverSetting != VoiceoverSetting.None;
            var voice = SettingsManager.Instance.CurrentVoiceoverSetting;

            #region Greeting
            yield return new WaitForSecondsRealtime(greetingAudioDelay);
            PlayGreeting(playVoiceover, voice);
            yield return new WaitUntil(() => !_audioSource.isPlaying && !SubtitleDisplayer.IsDisplaying);
            greetingComplete = true;
            #endregion

            #region Antenna POI
            AntennaNode.TogglePrompt(true);
            yield return new WaitUntil(() => AntennaNode.IsActivated);
            ActivateAntennaAndTriggerSunset(playVoiceover, voice);
            yield return new WaitUntil(() => !_audioSource.isPlaying && SunsetController.SunsetCompleted && !SubtitleDisplayer.IsDisplaying);
            antennaPOIComplete = true;
            #endregion

            #region Wavelength change
            yield return new WaitForSecondsRealtime(changeWavelengthAudioDelay);
            AntennaNode.Deactivate();
            PromptForWavelengthChange(playVoiceover, voice);
            yield return new WaitUntil(() => SpectrumStateController.Instance.CurrentWavelength == Wavelength.Radio);
            DisablePromptAndSkipButton();
            wavelengthChangeComplete = true;
            #endregion

            #region Radio Introduction
            yield return new WaitUntil(() => !_audioSource.isPlaying && !SubtitleDisplayer.IsDisplaying);
            yield return new WaitForSecondsRealtime(radioIntroDelay);
            PlayRadioIntro(playVoiceover, voice);
            yield return new WaitUntil(() => !_audioSource.isPlaying && !SubtitleDisplayer.IsDisplaying);
            radioIntroComplete = true;
            #endregion

            #region Sky POI Prompt
            SwitchToExplorationMode();
            yield return new WaitUntil(() => firstPOIActivated);
            
            #endregion

            SetSequenceComplete();
            yield break;
        }

        #region Coroutine Methods
        private void PlayGreeting(bool playVoiceover, VoiceoverSetting voice)
        {
            if (playVoiceover)
            {
                switch(SettingsManager.Instance.CurrentLanguageSetting)
                {
                    case LanguageSetting.English:
                        PlayAudioAndDisplaySubtitles(voice == VoiceoverSetting.Female ? GreetingEngFemale : GreetingEngMale, GreetingEngSubtitle);
                        break;
                    case LanguageSetting.Italian:
                        PlayAudioAndDisplaySubtitles(voice == VoiceoverSetting.Female ? GreetingItaFemale : GreetingItaMale, GreetingItaSubtitle);
                        break;
                }
                
            }
        }

        private void ActivateAntennaAndTriggerSunset(bool playVoiceover, VoiceoverSetting voice)
        {
            if (playVoiceover)
            {
                switch(SettingsManager.Instance.CurrentLanguageSetting)
                {
                    case LanguageSetting.English:
                        PlayAudioAndDisplaySubtitles(voice == VoiceoverSetting.Female ? AntennaEngFemale : AntennaEngMale, AntennaEngSubtitle);
                        break;
                    case LanguageSetting.Italian:
                        PlayAudioAndDisplaySubtitles(voice == VoiceoverSetting.Female ? AntennaItaFemale : AntennaItaMale, AntennaItaSubtitle);
                        break;
                }
                
            }
            SunsetController.StartSunset();
            SpectrumStateController.Instance.SetSpectrumStateToVisible();
        }

        private void PromptForWavelengthChange(bool playVoiceover, VoiceoverSetting voice)
        {
            if (playVoiceover)
            {
                switch (SettingsManager.Instance.CurrentLanguageSetting)
                {
                    case LanguageSetting.English:
                        PlayAudioAndDisplaySubtitles(voice == VoiceoverSetting.Female ? WavelengthEngFemale : WavelengthEngMale, WavelengthEngSubtitle);
                        break;
                    case LanguageSetting.Italian:
                        PlayAudioAndDisplaySubtitles(voice == VoiceoverSetting.Female ? WavelengthItaFemale : WavelengthItaMale, WavelengthItaSubtitle);
                        break;
                }
                
            }
            SpectrumCanvas.SetVisibleAndInteractableState(true);
            SpectrumCanvas.SetSpectrumSliderPanelVisible(true);
            RadioPrompt.TogglePrompt(true);
            SpectrumCanvas.SetRadioPipActive();
        }

        private void DisablePromptAndSkipButton()
        {
            RadioPrompt.TogglePrompt(false);
            SkipIntroButton.SetVisibleAndInteractableState(false);
        }

        private void PlayRadioIntro(bool playVoiceover, VoiceoverSetting voice)
        {
            if (playVoiceover)
            {
                switch (SettingsManager.Instance.CurrentLanguageSetting)
                {
                    case LanguageSetting.English:
                        PlayAudioAndDisplaySubtitles(voice == VoiceoverSetting.Female ? RadioIntroEngFemale : RadioIntroEngMale, RadioIntroEngSubtitle);
                        break;
                    case LanguageSetting.Italian:
                        PlayAudioAndDisplaySubtitles(voice == VoiceoverSetting.Female ? RadioIntroItaFemale : RadioIntroItaMale, RadioIntroItaSubtitle);
                        break;
                }
                
            }
        }

        private void SwitchToExplorationMode()
        {
            ToggleSkyPOIPrompt(true);
            SettingsManager.Instance.SetExperienceMode(ExperienceMode.Exploration);
            EventManager.Instance.AddListener<POINodeActivatedEvent>(HandleFirstActivatedNodeEvent);
        }

        private void SetSequenceComplete()
        {
            ToggleSkyPOIPrompt(false);
            sequenceComplete = true;
            isPlaying = false;
            EventManager.Instance.Raise(new IntroductionSequenceStateChangedEvent(IntroductionSequenceStateChangedEvent.IntroState.Complete, $"Introduction sequence complete."));
        }

        private void ToggleSkyPOIPrompt(bool visible)
        {
            SkyPOIPrompt.GetComponentsInChildren<Renderer>().ToList().ForEach(r => r.enabled = visible);
        }
        
        private void PlayAudioAndDisplaySubtitles(AudioClip audioClip, SubtitleData subtitle)
        {
            if (SettingsManager.Instance.CurrentVoiceoverSetting != VoiceoverSetting.None)
            {
                _audioSource.clip = audioClip;
                _audioSource.Play();
            }

            if (SettingsManager.Instance.ShowSubtitles)
            {
                SubtitleDisplayer.SetSubtitleQueue(subtitle);
            }
        }

        private void HandleFirstActivatedNodeEvent(POINodeActivatedEvent e)
        {
            firstPOIActivated = true;
            EventManager.Instance.RemoveListener<POINodeActivatedEvent>(HandleFirstActivatedNodeEvent);
        }
        #endregion
        
        public void StopIntroductionSequenece()
        {
            if (SettingsManager.Instance.CurrentExperienceMode == ExperienceMode.Introduction && introductionRoutine != null)
            {
                CameraBlink.EyeClosed.AddListener(StopCoroutineAndResetState);
                CameraBlink.Blink();
            }
        }

        public void StopCoroutineAndResetState()
        {
            ResetStateVariables();

            if (introductionRoutine != null)
            {
                StopCoroutine(introductionRoutine);
                introductionRoutine = null;
            }

            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }

            if (SubtitleDisplayer.IsDisplaying)
            {
                SubtitleDisplayer.StopSubtitles();
            }

            if (!SunsetController.SunsetCompleted)
            {
                SunsetController.ForceEndSunset();
            }

            SpectrumStateController.Instance.SetSpectrumStateToVisible();
            AntennaNode.Deactivate();
            AntennaNode.TogglePrompt(false);
            DisablePromptAndSkipButton();

            sequenceComplete = true;

            CameraBlink.EyeClosed.RemoveListener(StopCoroutineAndResetState);
            EventManager.Instance.Raise(new IntroductionSequenceStateChangedEvent(IntroductionSequenceStateChangedEvent.IntroState.Skipped, $"Introduction sequence completed (skip button used)."));
        }

        private void ResetStateVariables()
        {
            isPlaying = false;
            greetingComplete = false;
            antennaPOIComplete = false;
            wavelengthChangeComplete = false;
            radioIntroComplete = false;
            firstPOIActivated = false;
            sequenceComplete = false;
        }

        private void SetAndCheckReferences()
        {
            _audioSource = GetComponent<AudioSource>();
            Assert.IsNotNull(_audioSource, $"<b>[{GetType().Name}]</b> has no Audio Source component.");

            Assert.IsFalse(greetingAudioDelay < 0, $"<b>[{GetType().Name}]</b> greeting audio delay is less than zero.");
            Assert.IsNotNull(GreetingEngFemale, $"<b>[{GetType().Name}]</b> greeting clip female is not assigned.");
            Assert.IsNotNull(GreetingEngMale, $"<b>[{GetType().Name}]</b> greeting clip male is not assigned.");
            Assert.IsNotNull(GreetingEngSubtitle, $"<b>[{GetType().Name}]</b> greeting clip subtitle is not assigned.");

            Assert.IsNotNull(AntennaNode, $"<b>[{GetType().Name}]</b> antenna node is not assigned.");
            Assert.IsNotNull(AntennaEngSubtitle, $"<b>[{GetType().Name}]</b> antenna subtitle is not assigned.");
            Assert.IsNotNull(AntennaEngFemale, $"<b>[{GetType().Name}]</b> Antenna clip female is not assigned");
            Assert.IsNotNull(AntennaEngMale, $"<b>[{GetType().Name}]</b> Antenna clip male is not assigned");

            Assert.IsFalse(changeWavelengthAudioDelay < 0, $"<b>[{GetType().Name}]</b> change wavelength audio delay is less than zero.");
            Assert.IsNotNull(WavelengthEngFemale, $"<b>[{GetType().Name}]</b> change wavelength clip female is not assigned.");
            Assert.IsNotNull(WavelengthEngMale, $"<b>[{GetType().Name}]</b> change wavelength clip male is not assigned.");
            Assert.IsNotNull(WavelengthEngSubtitle, $"<b>[{GetType().Name}]</b> change wavelength subtitle is not assigned.");
            Assert.IsNotNull(RadioPrompt, $"<b>[{GetType().Name}]</b> radio prompt is not assigned and cannot be found in the scene.");

            Assert.IsFalse(radioIntroDelay < 0, $"<b>[{GetType().Name}]</b> radio intro audio delay is less than zero.");
            Assert.IsNotNull(RadioIntroEngFemale, $"<b>[{GetType().Name}]</b> radio intro clip female is not assigned.");
            Assert.IsNotNull(RadioIntroEngMale, $"<b>[{GetType().Name}]</b> radio intro clip female is not assigned.");
            Assert.IsNotNull(RadioIntroEngSubtitle, $"<b>[{GetType().Name}]</b> radio intro subtitle is not assigned.");
            
            Assert.IsNotNull(SkyPOIPrompt, $"<b>[{GetType().Name}]</b> Sky POI prompt game object is not assigned.");
            
            if (SubtitleDisplayer == null)
            {
                SubtitleDisplayer = FindObjectOfType<SubtitleDisplayer>();
            }
            Assert.IsNotNull(SubtitleDisplayer, $"<b>[{GetType().Name}]</b> subtitle displayer is not assigned and cannot be found in the scene.");

            if (SunsetController == null)
            {
                SunsetController = FindObjectOfType<SunsetController>();
            }
            Assert.IsNotNull(SunsetController, $"<b>[{GetType().Name}]</b> sunset controller is not assigned and cannot be found in the scene.");

            if (CameraBlink == null)
            {
                //CameraBlink = Camera.main.GetComponentInChildren<CameraBlink>();
                CameraBlink = Camera.main.GetComponentInChildren<CameraBlinkCanvas>();
            }
            Assert.IsNotNull(CameraBlink, $"<b>[{GetType().Name}]</b> Camera blink is not assigned and cannot be found in scene.");

            if (AntennaInfoPanel == null)
            {
                AntennaInfoPanel = FindObjectOfType<InfoPanelAntenna>();
            }
            Assert.IsNotNull(AntennaInfoPanel, $"<b>[{GetType().Name}]</b> Antenna info panel is not assigned and cannot be found in the scene.");

            if (SpectrumCanvas == null)
            {
                SpectrumCanvas = FindObjectOfType<SpectrumCanvas>();
            }
            Assert.IsNotNull(SpectrumCanvas, $"<b>[{GetType().Name}]</b> Spectrum canvas is not assigned and cannot be found in the scene.");

            if (SkipIntroButton == null)
            {
                SkipIntroButton = FindObjectOfType<SkipIntroButton>();
            }
            Assert.IsNotNull(SkipIntroButton, $"<b>[{GetType().Name}]</b> Skip intro button is not assigned and cannot be found in the scene.");
        }
    }
}