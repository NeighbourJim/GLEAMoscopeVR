using GLEAMoscopeVR.POIs;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Interaction
{
    /// <summary>
    /// Used to close <see cref="InfoPanelWarTable"/> and <see cref="InfoPanelSky"/>.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CloseInfoPanelButton : MonoBehaviour, IActivatable
    {
        [Space]
        public InfoPanelBase ParentPanel;
        [HideInInspector]
        public MaximiseButton MaximiseButton;

        [Header("IActivatable")]
        [SerializeField] private float activationTime = 1f;
        [SerializeField] private bool isActivated = false;
        [SerializeField] private bool canActivate = true;
        
        float IActivatable.ActivationTime => activationTime;
        bool IActivatable.IsActivated => isActivated;

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
                if (!(ParentPanel is InfoPanelSky))
                {
                    MaximiseButton.SetVisibleAndInteractableState(true);
                }
                ParentPanel.SetCanvasGroupAndColliderState(false);
            }
        }

        public void SetVisibleAndInteractableState(bool interactable)
        {
            _canvasGroup.alpha = interactable ? 1 : 0;
            _collider.enabled = interactable;
        }

        private void SetAndCheckReferences()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            Assert.IsNotNull(_canvasGroup, $"<b>[{GetType().Name}]</b> has no Canvas Group component.");

            _collider = GetComponent<Collider>();
            Assert.IsNotNull(_collider, $"<b>[{GetType().Name}]</b> has no collider component.");
            
            ParentPanel = GetComponentInParent<InfoPanelBase>();
            Assert.IsNotNull(ParentPanel, $"<b>[{GetType().Name}]</b> cannot find ParentPanel component in parent game object");
            
            MaximiseButton = FindObjectOfType<MaximiseButton>();
            Assert.IsNotNull(MaximiseButton, $"<b>[{GetType().Name}]</b> cannot find maximise button component in scene.");
        }
    }
}