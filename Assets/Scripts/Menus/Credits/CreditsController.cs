using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Menu
{
    public class CreditsController : MonoBehaviour
    {
        [Header("Canvas Objects")] [Tooltip("The canvases to fade through. They will fade in order.")]
        [SerializeField] private Canvas[] canvases;

        [Space] public CreditsBackButton BackButton;

        [Header("Fade Durations")]
        [Tooltip("The time in seconds it will take for a canvas to fade up")]
        [SerializeField] private float fadeUpTime = 1.5f;
        [Tooltip("The time in seconds it will take for a canvas to fade down")]
        [SerializeField] private float fadeDownTime = 1.5f;
        [Tooltip("The time in seconds between a canvas fading down and the next fading up.")]
        [SerializeField] private float waitTime = 0.5f;
        [Tooltip("The time in seconds a canvas will remain fully visible before fading to the next.")]
        [SerializeField] private float appearTime = 12f;

        [Space] public SlidePositionDisplayer SlidePositionDisplayer;
        [Space] public StartAreaManager StartAreaManager;

        Coroutine credits = null;
        bool fading = false;

        private void Start()
        {
            SetAndCheckReferences();
            SetAllCanvasGroupAlphaValues(false);
        }

        public void StartCredits()
        {
            credits = StartCoroutine(Credits());
        }

        public void StopCredits()
        {
            SlidePositionDisplayer.StopSlideRoutine();
            StopCoroutine(credits);
            SetAllCanvasGroupAlphaValues(false);
        }

        private void SetAllCanvasGroupAlphaValues(bool visible)
        {
            foreach (var canvas in canvases)
            {
                canvas.GetComponent<CanvasGroup>().alpha = visible ? 1 : 0;
            }
        }

        IEnumerator Credits()
        {
            BackButton.SetVisibleAndInteractableState(true);
            Coroutine fadeUp = null;
            Coroutine fadeDown = null;
            for (int i = 0; i < canvases.Length; i++)
            {

                if (i > 0)
                {
                    if (fadeDown != null)
                    {
                        StopCoroutine(fadeDown);
                    }

                    fadeDown = StartCoroutine(FadeCanvasDown(canvases[i - 1]));
                }

                yield return new WaitUntil(() => !fading);
                yield return new WaitForSeconds(waitTime);
                if (i < canvases.Length)
                {
                    if (fadeUp != null)
                    {
                        StopCoroutine(fadeUp);
                    }

                    fadeUp = StartCoroutine(FadeCanvasUp(canvases[i]));
                }

                yield return new WaitUntil(() => !fading);
                yield return new WaitForSeconds(appearTime);
                if (i == canvases.Length - 1)
                {
                    fadeDown = StartCoroutine(FadeCanvasDown(canvases[i]));
                    yield return new WaitUntil(() => !fading);
                }
            }

            canvases.ToList().ForEach(c => c.GetComponent<CanvasGroup>().alpha = 0);
            BackButton.SetVisibleAndInteractableState(false);
            StopCredits();
            StartAreaManager.ShowMainCanvas();

            yield break;
        }

        IEnumerator FadeCanvasUp(Canvas c)
        {
            float elapsed = 0;
            fading = true;
            while (elapsed < fadeUpTime)
            {
                c.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0f, 1f, (elapsed / fadeUpTime));
                elapsed += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForEndOfFrame();
            c.GetComponent<CanvasGroup>().alpha = 1;
            fading = false;
        }

        IEnumerator FadeCanvasDown(Canvas c)
        {
            float elapsed = 0;
            fading = true;
            while (elapsed < fadeDownTime)
            {
                c.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, (elapsed / fadeDownTime));
                elapsed += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForEndOfFrame();
            c.GetComponent<CanvasGroup>().alpha = 0;
            fading = false;
        }

        private void SetAndCheckReferences()
        {
            Assert.IsTrue(canvases.Length > 0, "Credits canvas array not set!");

            Assert.IsNotNull(SlidePositionDisplayer,
                $"<b>[{GetType().Name}]</b> has no reference to SlidePositionDisplayer component");

            Assert.IsNotNull(BackButton, $"<b>[{GetType().Name}]</b> Back button is not assigned.");

            if (StartAreaManager == null)
            {
                StartAreaManager = FindObjectOfType<StartAreaManager>();
            }
            Assert.IsNotNull(StartAreaManager, $"<b>[{GetType().Name}]</b> Start area manager is not assigned and cannot be found in scene.");
        }
    }
}