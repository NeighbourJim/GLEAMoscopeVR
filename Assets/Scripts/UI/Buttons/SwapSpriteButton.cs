using GLEAMoscopeVR.POIs;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Interaction
{
    /// <summary>
    /// Provide more control over sprite cycling to assist with POIs that don't exist in more than one sprite (Antenna)
    /// </summary>
    public class SwapSpriteButton : ActivatableButton
    {
        public enum Direction { Shorter, Longer }

        [SerializeField] private Direction direction;
        [SerializeField] private float activationTime = 0.5f;
        [SerializeField] private bool isActivated = false;

        public override float ActivationTime => activationTime;

        public override bool IsActivated => isActivated;

        #region References
        InfoPanel _infoPanel;
        CanvasGroup _canvasGroup;
        #endregion

        #region Unity Methods
        void Awake()
        {
            SetAndCheckReferences();
        }
        #endregion

        public override bool CanActivate() => _infoPanel.CanCycleSprites;

        public override void Activate()
        {
            if (CanActivate())
            {
                if (direction == Direction.Shorter)
                {
                    _infoPanel.CycleSpriteLeft();
                }
                else
                {
                    _infoPanel.CycleSpriteRight();
                }
            }
        }

        public override void Deactivate() { }
        
        #region Debugging

        private void SetAndCheckReferences()
        {
            _canvasGroup = GetComponentInParent<CanvasGroup>();
            Assert.IsNotNull(_canvasGroup, $"{gameObject.name} cannot find CanvasGroup component in parent game object.");

            _infoPanel = GetComponentInParent<InfoPanel>();
            Assert.IsNotNull(_infoPanel, $"{gameObject.name} cannot find InfoPanel component in parent game object");
        }
        
        #endregion
    }
}