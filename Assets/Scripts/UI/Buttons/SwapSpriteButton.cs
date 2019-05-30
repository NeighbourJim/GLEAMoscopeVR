using GLEAMoscopeVR.POIs;
using GLEAMoscopeVR.Spectrum;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Interaction
{
    /// <summary>
    /// Provide more control over sprite cycling to assist with POIs that don't exist in more than one sprite (Antenna)
    /// </summary>
    public class SwapSpriteButton : MonoBehaviour, IActivatable
    {
        private const string soundEffect = "switch";

        [SerializeField]
        private SpectrumDirection direction;
        [SerializeField]
        private float activationTime = 1f;
        [SerializeField]
        private bool isActivated = false;

        float IActivatable.ActivationTime => activationTime;

        bool IActivatable.IsActivated => isActivated;

        #region References
        InfoPanel _infoPanel;
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
                if (direction == SpectrumDirection.Shorter)
                {
                    _infoPanel.CycleSpriteLeft();
                }
                else
                {
                    _infoPanel.CycleSpriteRight();
                }
            }
        }

        void IActivatable.Deactivate() { }
        
        #region Debugging

        private void SetAndCheckReferences()
        {
            _canvasGroup = GetComponentInParent<CanvasGroup>();
            Assert.IsNotNull(_canvasGroup, $"{gameObject.name} cannot find CanvasGroup component in parent game object.");

            _infoPanel = GetComponentInParent<InfoPanel>();
            Assert.IsNotNull(_infoPanel, $"{gameObject.name} cannot find InfoPanel component in parent game object");

            _soundEffects = FindObjectOfType<SoundEffects>();
            Assert.IsNotNull(_soundEffects, $"{gameObject.name} cannot find SoundEffects in scene.");
        }
        
        #endregion
    }
}