using System.Linq;
using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Settings
{
    public class WarTableSettingsPanel : MonoBehaviour, IHideableUI
    {
        public SpriteRenderer BackgroundSpriteRenderer;
        public CloseButton CloseButton;
        public ReplayIntroductionButton ReplayIntroductionButton;

        private bool isActive = false;
        public bool IsActive => isActive;

        public CanvasGroup CanvasGroup => GetComponent<CanvasGroup>();
        public Collider Collider => null;

        void Start()
        {
            SetAndCheckReferences();
            SetVisibleAndInteractableState(false);
        }

        void OnEnable()
        {
            EventManager.Instance.AddListener<ExperienceModeChangedEvent>(_ => SetVisibleAndInteractableState(false));
        }
        void OnDisable()
        {
            EventManager.Instance.RemoveListener<ExperienceModeChangedEvent>(_ => SetVisibleAndInteractableState(false));
        }

        public void SetVisibleAndInteractableState(bool visible)
        {
            isActive = visible;
            CanvasGroup.alpha = visible ? 1 : 0;
            BackgroundSpriteRenderer.enabled = visible;
            CloseButton.SetVisibleAndInteractableState(visible);
            ReplayIntroductionButton.SetVisibleAndInteractableState(visible && SettingsManager.Instance.CurrentExperienceMode != ExperienceMode.Introduction);
            GetComponentsInChildren<Collider>().ToList().ForEach(c => c.enabled = visible);
        }

        private void SetAndCheckReferences()
        {
            Assert.IsNotNull(BackgroundSpriteRenderer, $"<b>[{GetType().Name}]</b> Background sprite renderer is not assigned.");
            Assert.IsNotNull(CanvasGroup, $"<b>[{GetType().Name}]</b> has no canvas group.");
            Assert.IsNotNull(ReplayIntroductionButton, $"<b>[{GetType().Name}]</b> Replay introduction button is not assigned.");
        }
    }
}