using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Menu
{
    public class CreditsBackButton : MonoBehaviour, IActivatable, IHideableUI
    {
        [Header("IActivatable")]
        [SerializeField] private float activationTime = 2f;
        [SerializeField] private bool canActivate = true;

        [Space]
        public SlidePositionDisplayer SlidePositionDisplayer;
        public CreditsController CreditsController;

        [Space] public StartAreaManager StartAreaManager;

        #region References
        CanvasGroup _canvasGroup;
        Collider _collider;
        #endregion

        float IActivatable.ActivationTime => activationTime;
        bool IActivatable.IsActivated => false;
        CanvasGroup IHideableUI.CanvasGroup => _canvasGroup;
        Collider IHideableUI.Collider => _collider;

        #region Unity Methods
        void Awake()
        {
            SetAndCheckReferences();
        }
        #endregion

        bool IActivatable.CanActivate()
        {
            return canActivate;
        }

        void IActivatable.Activate()
        {
            CreditsController.StopCredits();
            StartAreaManager.ShowMainCanvas();
            SlidePositionDisplayer.StopSlideRoutine();
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
            Assert.IsNotNull(_canvasGroup, $"<b>[BackButton]</b> has no Canvas Group component in parent.");

            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_collider, $"<b>[BackButton]</b> has no collider component.");

            CreditsController = GetComponentInParent<CreditsController>();
            
            Assert.IsNotNull(SlidePositionDisplayer, $"<b>[{GetType().Name}]</b> has no reference to SlidePositionDisplayer component");

            if (StartAreaManager == null)
            {
                StartAreaManager = FindObjectOfType<StartAreaManager>();
            }
            Assert.IsNotNull(StartAreaManager, $"<b>[{GetType().Name}]</b> Start area manager is not assigned and cannot be found in scene.");
        }
    }
}