using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using GLEAMoscopeVR.POIs;
using GLEAMoscopeVR.Spectrum;

namespace GLEAMoscopeVR.Interaction
{
    public class CloseToolTipButton : MonoBehaviour, IActivatable
    {
        private const string soundEffect = "switch";

        [SerializeField]
        private float activationTime = 1f;
        [SerializeField]
        private bool isActivated = false;

        float IActivatable.ActivationTime => activationTime;

        bool IActivatable.IsActivated => isActivated;

        #region References
        InfoPanel_Tooltip _infoPanel;
        CanvasGroup _canvasGroup;
        SoundEffects _soundEffects;
        #endregion

        #region Unity Methods
        void Awake()
        {
            SetAndCheckReferences();
        }
        #endregion

        bool IActivatable.CanActivate() => _infoPanel.CanCycleSprites;

        void IActivatable.Activate()
        {
            if (this is IActivatable activatable && activatable.CanActivate())
            {
                _infoPanel.SetCanvasGroupAndColliderState(false);
            }
        }

        void IActivatable.Deactivate() { }

        #region Debugging

        private void SetAndCheckReferences()
        {
            _canvasGroup = GetComponentInParent<CanvasGroup>();
            Assert.IsNotNull(_canvasGroup, $"{gameObject.name} cannot find CanvasGroup component in parent game object.");

            _infoPanel = GetComponentInParent<InfoPanel_Tooltip>();
            Assert.IsNotNull(_infoPanel, $"{gameObject.name} cannot find InfoPanel component in parent game object");

            _soundEffects = FindObjectOfType<SoundEffects>();
            Assert.IsNotNull(_soundEffects, $"{gameObject.name} cannot find SoundEffects in scene.");
        }

        #endregion
    }
}
