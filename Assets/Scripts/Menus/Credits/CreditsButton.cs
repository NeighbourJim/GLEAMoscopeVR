using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Menu
{
    public class CreditsButton : MonoBehaviour, IActivatable, IHideableUI
    {
        [Header("IActivatable")]
        [SerializeField] private float activationTime = 2f;
        [SerializeField] private bool canActivate = false;

        [Space]
        public CreditsController CreditsController = null;
        public SlidePositionDisplayer SlidePositionDisplayer;

        [Space] public StartAreaManager StartAreaManager;

        #region References
        CanvasGroup _canvasGroup;
        Collider _collider;
        #endregion

        CanvasGroup IHideableUI.CanvasGroup => _canvasGroup;
        Collider IHideableUI.Collider => _collider;
        float IActivatable.ActivationTime => activationTime;
        bool IActivatable.IsActivated => false;

        #region Unity Methods
        void Awake()
        {
            SetAndCheckReferences();
        }
        #endregion

        bool IActivatable.CanActivate() => canActivate;

        void IActivatable.Activate()
        {
            StartAreaManager.ShowCreditsCanvas();
            CreditsController.StartCredits();
            SlidePositionDisplayer.StartSlideRoutine();
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

            if (CreditsController == null)
            {
                CreditsController = FindObjectOfType<CreditsController>();
            }
            Assert.IsNotNull(CreditsController, $"<b>[{GetType().Name}]</b> Credits controller is not assigned and cannot be found in scene.");

            Assert.IsNotNull(SlidePositionDisplayer, $"<b>[{GetType().Name}]</b> has no reference to SlidePositionDisplayer component");

            if (StartAreaManager == null)
            {
                StartAreaManager = FindObjectOfType<StartAreaManager>();
            }
            Assert.IsNotNull(StartAreaManager, $"<b>[{GetType().Name}]</b> Start area manager is not assigned and cannot be found in scene.");
        }
    }
}