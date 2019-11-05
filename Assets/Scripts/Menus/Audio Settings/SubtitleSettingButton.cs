using GLEAMoscopeVR.Events;
using UnityEngine;
using UnityEngine.Assertions;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.UI;
using UnityEngine.UI;

namespace GLEAMoscopeVR.Settings
{
    public class SubtitleSettingButton : MonoBehaviour, IActivatable, IHideableUI
    {
        public Image CheckedImage;

        [Header("IActivatable")]
        [SerializeField] private float activationTime = 2f;
        [SerializeField] private bool canActivate = true;

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

        void Start()
        {
            SetCheckedState();
        }

        void OnEnable()
        {
            EventManager.Instance.AddListener<SubtitleSettingChangedEvent>(_ => SetCheckedState());
        }

        void OnDisable()
        {
            EventManager.Instance.RemoveListener<SubtitleSettingChangedEvent>(_ => SetCheckedState());
        }
        #endregion

        public void SetCheckedState()
        {
            CheckedImage.enabled = SettingsManager.Instance.UserSettings.ShowSubtitles;
        }

        bool IActivatable.CanActivate()
        {
            return canActivate;
        }

        void IActivatable.Activate()
        {
            SettingsManager.Instance.SetSubtitleSetting(!SettingsManager.Instance.UserSettings.ShowSubtitles);
            SetCheckedState();
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

            Assert.IsNotNull(CheckedImage, $"<b>[{GetType().Name}]</b> Checked image is not assigned.");
        }
    }
}