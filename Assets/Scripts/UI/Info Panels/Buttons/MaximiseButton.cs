using GLEAMoscopeVR.POIs;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Interaction
{
    /// <summary>
    /// Used to close both the the war table and sky tooltip panels (<see cref="InfoPanelBase"/>).
    /// May also be used for settings panel
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Collider))]
    public class MaximiseButton : MonoBehaviour, IActivatable
    {
        public InfoPanelWarTable WarTablePanel;

        [Header("IActivatable")]
        [SerializeField]
        private float activationTime = 1f;
        [SerializeField]
        private bool isActivated = false;
        [SerializeField]
        private bool canActivate = true;
        float IActivatable.ActivationTime => activationTime;
        public bool IsActivated => isActivated;
        
        #region References
        CanvasGroup _canvasGroup;
        Collider _collider;
        #endregion

        #region Unity Methods
        void Awake()
        {
            SetAndCheckReferences();
        }
        #endregion

        bool IActivatable.CanActivate() => canActivate;

        void IActivatable.Activate()
        {
            if (this is IActivatable activatable && activatable.CanActivate())
            {
                WarTablePanel.SetCanvasGroupAndColliderState(true);
                SetVisibleAndInteractableState(false);
            }
        }

        public void SetVisibleAndInteractableState(bool interactable)
        {
            isActivated = interactable;
            _canvasGroup.alpha = interactable ? 1 : 0;
            _collider.enabled = interactable;
        }

        private void SetAndCheckReferences()
        {
            _canvasGroup = GetComponentInParent<CanvasGroup>();
            Assert.IsNotNull(_canvasGroup, $"<b>[{GetType().Name}]</b> has no Canvas Group component.");

            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_collider, $"<b>[{GetType().Name}]</b> has no collider component.");

            if (WarTablePanel == null)
            {
                WarTablePanel = FindObjectOfType<InfoPanelWarTable>();
            }
            Assert.IsNotNull(WarTablePanel, $"<b>[{GetType().Name}]</b> war table panel is not assigned and cannot be found in the scene.");
        }
    }
}