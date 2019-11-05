﻿using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Menu
{
    public class AcknowledgementsBackButton : MonoBehaviour, IActivatable, IHideableUI
    {
        [Header("IActivatable")]
        [SerializeField] private float activationTime = 2f;
        [SerializeField] private bool canActivate = true;

        [Space]
        public SlidePositionDisplayer SlidePositionDisplayer;
        public AcknowledgementsController AcknowledgementsController;

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
        
        bool IActivatable.CanActivate()
        {
            return canActivate;
        }

        void IActivatable.Activate()
        {
            StartAreaManager.ShowMainCanvas();
            AcknowledgementsController.StopAcknowledgementsCycle();
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
            Assert.IsNotNull(_canvasGroup, $"<b>[{GetType().Name}]</b> has no Canvas Group component in parent.");

            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_collider, $"<b>[{GetType().Name}]</b> has no collider component.");

            if (AcknowledgementsController == null)
            {
                AcknowledgementsController = FindObjectOfType<AcknowledgementsController>();
            }
            Assert.IsNotNull(AcknowledgementsController, $"[{GetType().Name}] Acknowledgements controller is not assigned and cannot be found in the scene.");

            Assert.IsNotNull(SlidePositionDisplayer, $"<b>[{GetType().Name}]</b> has no reference to SlidePositionDisplayer component");

            if (StartAreaManager == null)
            {
                StartAreaManager = FindObjectOfType<StartAreaManager>();
            }
            Assert.IsNotNull(StartAreaManager, $"<b>[{GetType().Name}]</b> Start area manager is not assigned and cannot be found in scene.");
        }
    }
}