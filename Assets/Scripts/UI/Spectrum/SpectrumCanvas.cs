using System.Collections;
using System.Linq;
using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Settings;
using GLEAMoscopeVR.UI;
using GLEAMoscopeVR.Utility.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.Spectrum
{
    public class SpectrumCanvas : MonoBehaviour, IHideableUI
    {
        private const int pipCount = 6;

        [Header("Slider")]
        public Slider SpectrumSlider;
        public CanvasGroup SliderPanelCanvasGroup;

        [Header("Buttons")]
        public SpectrumButton ShorterButton;
        public SpectrumButton LongerButton;

        [Header("Text")]
        public TextMeshProUGUI SpectrumText;

        [Header("Pips")]
        public SpectrumPip[] SpectrumPips;

        /// <summary>
        /// Value is set based on <see cref="SpectrumStateController.FadeTime"/> in Start().
        /// </summary>
        private float slideTime = 1f;

        CanvasGroup _canvasGroup;
        Collider _collider;

        CanvasGroup IHideableUI.CanvasGroup => _canvasGroup;
        Collider IHideableUI.Collider => _collider;

        #region Unity Methods
        void Awake()
        {
            SetAndCheckReferences();
        }

        void Start()
        {
            SetVisibleAndInteractableState(false);
        }

        void OnEnable()
        {
            EventManager.Instance.AddListener<IntroductionSequenceStateChangedEvent>(HandleIntroductionSequenceStateChanged);
            EventManager.Instance.AddListener<SunsetStateChangedEvent>(HandleSunsetStateChanged);
            EventManager.Instance.AddListener<SpectrumStateChangedEvent>(e => SlideToState(e.Wavelength));
        }

        void OnDisable()
        {
            EventManager.Instance.RemoveListener<IntroductionSequenceStateChangedEvent>(HandleIntroductionSequenceStateChanged);
            EventManager.Instance.RemoveListener<SunsetStateChangedEvent>(HandleSunsetStateChanged);
            EventManager.Instance.RemoveListener<SpectrumStateChangedEvent>(e => SlideToState(e.Wavelength));
        }
        #endregion

        #region Event Handlers
        private void HandleIntroductionSequenceStateChanged(IntroductionSequenceStateChangedEvent e)
        {
            switch (e.State)
            {
                case IntroductionSequenceStateChangedEvent.IntroState.Playing:
                    SetSpectrumStateCanvasComponentState(false);
                    break;
                case IntroductionSequenceStateChangedEvent.IntroState.Complete:
                case IntroductionSequenceStateChangedEvent.IntroState.Skipped:
                    SetVisibleAndInteractableState(true);
                    SetSpectrumStateCanvasComponentState(true);
                    break;
                case IntroductionSequenceStateChangedEvent.IntroState.Inactive:
                default:
                    break;
            }
        }

        private void HandleSunsetStateChanged(SunsetStateChangedEvent e)
        {
            if (SettingsManager.Instance.CurrentExperienceMode != ExperienceMode.Introduction && e.State == EventState.Completed)
            {
                SetVisibleAndInteractableState(true);
                SetSpectrumStateCanvasComponentState(true);
            }
        }
        #endregion

        #region Slider Manipulation
        /// <summary>
        /// Starts the coroutines for animating the slider's handle.
        /// </summary>
        /// <param name="state">The state being transitioned towards.</param>
        private void SlideToState(Wavelength state)
        {
            var desiredSliderValue = ((int)state) * 20f;
            StartCoroutine(SliderToValue(desiredSliderValue));
        }

        IEnumerator SliderToValue(float desiredValue)
        {
            float elapsed = 0;

            while (elapsed < slideTime)
            {
                SpectrumSlider.value = Mathf.Lerp(SpectrumSlider.value, desiredValue, (elapsed / slideTime));
                elapsed += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForEndOfFrame();
            SpectrumSlider.value = desiredValue;
        }

        #endregion

        #region Visibility / Interaction
        public void SetSpectrumStateCanvasComponentState(bool active)
        {
            SpectrumPips.ToList().ForEach(p => { p.ToggleActivatableStatus(active); });
            ShorterButton.SetVisibleAndInteractableState(active);
            LongerButton.SetVisibleAndInteractableState(active);
            SetSpectrumSliderPanelVisible(active);
        }

        public void SetRadioPipActive()
        {
            SpectrumPips.ToList().ForEach(p => p.ToggleActivatableStatus(p.Wavelength == Wavelength.Radio));
        }

        public void SetSpectrumSliderPanelVisible(bool visible)
        {
            SliderPanelCanvasGroup.alpha = visible ? 1 : 0;
        }

        public void SetVisibleAndInteractableState(bool visible)
        {
            _canvasGroup.alpha = visible ? 1 : 0;
            GetComponentsInChildren<Collider>().ToList().ForEach(c => c.enabled = visible);
        }
        #endregion

        private void SetAndCheckReferences()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            Assert.IsNotNull(_canvasGroup, $"<b>[{GetType().Name}]</b> has no Canvas Group component.");

            Assert.IsNotNull(SpectrumSlider, $"<b>[{GetType().Name}]</b> Spectrum slider not assigned.");
            Assert.IsNotNull(SliderPanelCanvasGroup, $"<b>[{GetType().Name}]</b> Slider panel canvas group not assigned.");

            Assert.IsNotNull(SpectrumText, $"<b>[{GetType().Name}]</b> Spectrum text not assigned.");

            Assert.IsNotNull(ShorterButton, $"<b>[{GetType().Name}]</b> Shorter button not assigned.");
            Assert.IsNotNull(LongerButton, $"<b>[{GetType().Name}]</b> Longer button not assigned.");

            Assert.IsFalse(SpectrumPips.IsNullOrEmpty(), $"<b>[{GetType().Name}]</b> Spectrum pips array is null or empty.");
            Assert.IsTrue(SpectrumPips.Length == pipCount, $"<b>[{GetType().Name}]</b> Spectrum pips array length is {SpectrumPips.Length}. Should be 6.");
        }
    }
}