using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Menu
{
    public class StartButton : MonoBehaviour, IActivatable, IHideableUI
    {
        [Header("IActivatable")]
        [SerializeField] private float activationTime = 2f;
        [SerializeField] private bool canActivate = false;

        public StartAreaManager StartAreaManager;

        #region References
        CanvasGroup _parentCanvasGroup;
        Collider _collider;
        #endregion

        CanvasGroup IHideableUI.CanvasGroup => _parentCanvasGroup;
        Collider IHideableUI.Collider => _collider;
        float IActivatable.ActivationTime => activationTime;
        bool IActivatable.IsActivated => false;
        
        #region Unity Methods
        void Awake()
        {
            SetAndCheckReferences();
        }

        void OnEnable()
        {
            EventManager.Instance.AddListener<VideoClipEndedEvent>(_ => canActivate = true);
        }

        void OnDisable()
        {
            EventManager.Instance.RemoveListener<VideoClipEndedEvent>(_ => canActivate = true);
        }
        #endregion

        bool IActivatable.CanActivate()
        {
            return canActivate;
        }

        void IActivatable.Activate()
        {
            canActivate = false;
            StartAreaManager.StartTeleport();
            SetVisibleAndInteractableState(false);

            GvrCardboardHelpers.Recenter();
        }

        public void SetVisibleAndInteractableState(bool visible)
        {
            canActivate = visible;
            _parentCanvasGroup.alpha = visible ? 1 : 0;
            _collider.enabled = visible;
        }

        private void SetAndCheckReferences()
        {
            _parentCanvasGroup = GetComponentInParent<CanvasGroup>();
            Assert.IsNotNull(_parentCanvasGroup, $"<b>[{GetType().Name}]</b> Cannot find Canvas Group component in parent.");

            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_collider, $"<b>[{GetType().Name}]</b> has no collider component.");

            if (StartAreaManager == null)
            {
                StartAreaManager = FindObjectOfType<StartAreaManager>();
            }
            Assert.IsNotNull(StartAreaManager, $"<b>[{GetType().Name}]</b> Start area manager is not assigned and cannot be found in scene.");
        }
    }
}