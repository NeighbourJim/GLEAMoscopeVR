using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Events.GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GLEAMoscopeVR.Settings
{
    public class BlinkInPassiveButton : MonoBehaviour, IActivatable, IHideableUI
    {
        public Image CheckedImage;

        [Header("IActivatable")]
        [SerializeField] private bool isActivated = false;
        [SerializeField] private float activationTime = 2f;
        [SerializeField] private bool canActivate = true;
        
        #region References
        CanvasGroup _canvasGroup;
        Collider _collider;
        #endregion

        CanvasGroup IHideableUI.CanvasGroup => _canvasGroup;
        Collider IHideableUI.Collider => _collider;
        float IActivatable.ActivationTime => activationTime;
        bool IActivatable.IsActivated => isActivated;

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
            EventManager.Instance.AddListener<PassiveBlinkSettingChanged>(_ => SetCheckedState());
        }

        void OnDisable()
        {
            EventManager.Instance.RemoveListener<PassiveBlinkSettingChanged>(_ => SetCheckedState());
        }
        #endregion

        public void SetCheckedState()
        {
            isActivated = SettingsManager.Instance.BlinkInPassiveMode;
            CheckedImage.enabled = isActivated;
        }

        bool IActivatable.CanActivate()
        {
            return canActivate;
        }

        void IActivatable.Activate()
        {
            SettingsManager.Instance.SetBlinkInPassiveMode(!SettingsManager.Instance.BlinkInPassiveMode);
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