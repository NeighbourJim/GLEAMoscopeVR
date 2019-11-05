using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Settings
{
    public class ReplayIntroductionButton : MonoBehaviour, IActivatable, IHideableUI
    {
        [Header("IActivatable")]
        [SerializeField] private float activationTime = 2f;
        [SerializeField] private bool canActivate = false;
        
        CanvasGroup _canvasGroup;
        Collider _collider;
        
        float IActivatable.ActivationTime => activationTime;
        bool IActivatable.IsActivated => false;
        CanvasGroup IHideableUI.CanvasGroup => _canvasGroup;
        Collider IHideableUI.Collider => _collider;

        void Start()
        {
            SetAndCheckReferences();
            SetVisibleAndInteractableState(false);
        }

        bool IActivatable.CanActivate()
        {
            return SettingsManager.Instance.CurrentExperienceMode != ExperienceMode.Introduction && canActivate;
        }

        void IActivatable.Activate()
        {
            if(SettingsManager.Instance.CurrentExperienceMode != ExperienceMode.Introduction && canActivate)
            {
                SettingsManager.Instance.SetExperienceMode(ExperienceMode.Introduction);
            }
        }

        public void SetVisibleAndInteractableState(bool visible)
        {
            var validMode = SettingsManager.Instance.CurrentExperienceMode != ExperienceMode.Introduction;
            canActivate = visible && validMode;

            _canvasGroup.alpha = visible && validMode ? 1 : 0;

            _collider.enabled = visible && validMode;
        }

        private void SetAndCheckReferences()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            Assert.IsNotNull(_canvasGroup, $"<b>[{GetType().Name}]</b> has no Canvas Group component.");

            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_canvasGroup, $"<b>[{GetType().Name}]</b> has no Collider component.");
        }
    }
}