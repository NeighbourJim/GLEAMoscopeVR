using System;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.Settings;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.UI
{
    public class CloseButton : MonoBehaviour, IActivatable, IHideableUI
    {
        public WarTableSettingsPanel ParentPanel;

        public CanvasGroup CanvasGroup => GetComponent<CanvasGroup>();
        public Collider Collider => GetComponent<Collider>();

        public float ActivationTime => 2f;
        public bool IsActivated => false;

        void Start()
        {
            SetAndCheckReferences();
            SetVisibleAndInteractableState(false);
        }

        public void Activate()
        {
            ParentPanel.SetVisibleAndInteractableState(false);
        }

        public bool CanActivate()
        {
            return Math.Abs(CanvasGroup.alpha - 1) < Mathf.Epsilon;
        }

        public void Deactivate(){}

        public void SetVisibleAndInteractableState(bool visible)
        {
            CanvasGroup.alpha = visible ? 1 : 0;
            Collider.enabled = visible;
        }

        private void SetAndCheckReferences()
        {
            Assert.IsNotNull(CanvasGroup, $"<b>[{GetType().Name}]</b> has no Canvas Group component.");
            Assert.IsNotNull(Collider, $"<b>[{GetType().Name}]</b> has no collider component.");

            ParentPanel = GetComponentInParent<WarTableSettingsPanel>();
            Assert.IsNotNull(ParentPanel, $"<b>[{GetType().Name}]</b> cannot find parent panel.");
        }
    }
}