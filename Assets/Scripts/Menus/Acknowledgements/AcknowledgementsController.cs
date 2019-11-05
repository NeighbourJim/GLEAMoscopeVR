using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Menu
{
    public class AcknowledgementsController : MonoBehaviour
    {
        [Header("Text Acknowledgements")]
        public TextMeshProUGUI TraditionalOwnersText;
        public TextMeshProUGUI AllSkyText;
        public TextMeshProUGUI CSIROText;
        public TextMeshProUGUI MWAText;
        public TextMeshProUGUI SKAOText;
        public TextMeshProUGUI ICRARText;

        [Header("Logo Acknowledgements")]
        public GameObject AcknowledgementsLogoPanel;

        [Header("Wait Times")]
        [SerializeField] private float standardDisplayTime = 7.5f;
        [SerializeField] private float longDisplayTime = 10f;

        [Space] public AcknowledgementsBackButton BackButton;
        [Space] public SlidePositionDisplayer SlidePositionDisplayer;
        [Space] public StartAreaManager StartAreaManager;


        [Header("Debugging")]
        [SerializeField] private bool isActive = false;
        [SerializeField] private bool isCyclingText = false;

        Coroutine displayAcknowledgementsRoutine;

        #region Unity Methods
        void Awake()
        {
            SetAndCheckReferences();
        }

        void Start()
        {
            SetComponentState(false);
        }
        #endregion

        private void SetComponentState(bool visible)
        {
            ToggleTextComponentStatus(visible);
            BackButton.SetVisibleAndInteractableState(visible);
        }

        public void DisplayAcknowledgements()
        {
            displayAcknowledgementsRoutine = StartCoroutine(CycleAcknowledgements());
        }

        public void StopAcknowledgementsCycle()
        {
            if (displayAcknowledgementsRoutine != null)
            {
                StopCoroutine(displayAcknowledgementsRoutine);
                displayAcknowledgementsRoutine = null;
                SetComponentState(false);
                isActive = false;
                isCyclingText = false;
            }
        }

        IEnumerator CycleAcknowledgements()
        {
            isActive = true;
            isCyclingText = true;

            BackButton.SetVisibleAndInteractableState(true);
            TraditionalOwnersText.enabled = true;

            yield return new WaitForSecondsRealtime(longDisplayTime);

            TraditionalOwnersText.enabled = false;
            AllSkyText.enabled = true;

            yield return new WaitForSecondsRealtime(longDisplayTime);

            AllSkyText.enabled = false;
            CSIROText.enabled = true;

            yield return new WaitForSecondsRealtime(standardDisplayTime);

            CSIROText.enabled = false;
            MWAText.enabled = true;

            yield return new WaitForSecondsRealtime(standardDisplayTime);

            MWAText.enabled = false;
            SKAOText.enabled = true;

            yield return new WaitForSecondsRealtime(standardDisplayTime);

            SKAOText.enabled = false;
            ICRARText.enabled = true;

            yield return new WaitForSecondsRealtime(longDisplayTime);

            ToggleTextComponentStatus(false);
            isCyclingText = false;

            AcknowledgementsLogoPanel.SetActive(true);

            yield return new WaitForSecondsRealtime(longDisplayTime);

            AcknowledgementsLogoPanel.SetActive(false);
            BackButton.SetVisibleAndInteractableState(false);
            isActive = false;
            StartAreaManager.ShowMainCanvas();
            SlidePositionDisplayer.StopSlideRoutine();
            yield break;
        }
        
        private void ToggleTextComponentStatus(bool display)
        {
            AcknowledgementsLogoPanel.SetActive(display);
            TraditionalOwnersText.enabled = display;
            AllSkyText.enabled = display;
            CSIROText.enabled = display;
            MWAText.enabled = display;
            SKAOText.enabled = display;
            ICRARText.enabled = display;
        }
        
        private void SetAndCheckReferences()
        {
            Assert.IsNotNull(AcknowledgementsLogoPanel, $"<b>[{GetType().Name}]</b> Acknowledgements logo panel is not assigned.");
            Assert.IsNotNull(TraditionalOwnersText, $"<b>[{GetType().Name}]</b> Traditional owners text is not assigned.");
            Assert.IsNotNull(AllSkyText, $"<b>[{GetType().Name}]</b> Allsky text is not assigned.");
            Assert.IsNotNull(CSIROText, $"<b>[{GetType().Name}]</b> CSIRO text is not assigned.");
            Assert.IsNotNull(MWAText, $"<b>[{GetType().Name}]</b> MWA text is not assigned.");
            Assert.IsNotNull(SKAOText, $"<b>[{GetType().Name}]</b> SKAO text is not assigned.");
            Assert.IsNotNull(ICRARText, $"<b>[{GetType().Name}]</b> ICRAR text is not assigned.");

            Assert.IsNotNull(BackButton, $"<b>[{GetType().Name}]</b> Back button is not assigned.");
            
            Assert.IsNotNull(SlidePositionDisplayer, $"<b>[{GetType().Name}]</b> has no reference to SlidePositionDisplayer component");

            if (StartAreaManager == null)
            {
                StartAreaManager = FindObjectOfType<StartAreaManager>();
            }
            Assert.IsNotNull(StartAreaManager, $"<b>[{GetType().Name}]</b> Start area manager is not assigned and cannot be found in scene.");
        }
    }
}