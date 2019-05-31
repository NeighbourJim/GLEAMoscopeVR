using System;
using GLEAMoscopeVR.Settings;
using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// War-table map point of interest nodes.
    /// Todo: MM - OnNodeDeactivated, set PassiveModeRotator to respond to OnNodeActivated event.
    /// </summary>
    public class POIMapNode : POINode
    {
        #region Constants
        private const ExperienceMode activatableMode = ExperienceMode.Passive;
        #endregion

        /// <summary>
        /// Point of Interest world-space transform. Used by <see cref="PassiveModeRotator"/>
        /// to bring the Point of Interest into view (when interacting in passive <see cref="ExperienceMode"/>).
        /// </summary>
        
        #region Public Accessors
        public override ExperienceMode ActivatableMode => activatableMode;
        public override bool IsActivated => isActivated;
        public override POIData Data => data;
        public override float ActivationTime => activationTime;
        #endregion

        #region Events
        /// <summary>
        /// Invoked when a node is activated.
        /// </summary>
        public override event Action<POINode> OnPOINodeActivated;
        #endregion

        #region References
        /// <summary>
        /// Reference to the <see cref="PassiveModeRotator"/> script used to rotate Points of interest into the user's original, forward-facing direction.
        /// </summary>
        PassiveModeRotator _rotator;
        #endregion

        #region IActivatable Implementation
        /// <summary> Specifies whether this GameObject can currently be activated. </summary>
        public override bool CanActivate()
        {
            return activatableMode == _modeController.CurrentMode && !isActivated && _rotator.CanSetRotationTarget();
        }

        /// <summary>
        /// Invoked by the <see cref="Script_CameraRayCaster"/> when the reticle loading process is complete.
        /// Triggers rotation of the Point of Interest into the user's view and notifies the <see cref="POIManager"/>
        /// that it has been activated.
        /// </summary>
        public override void Activate()
        {
            if (!CanActivate()) return;
            
            isActivated = true;
            _rotator.SetTargetTransformAndRotate(Data.SkyTransform);
            OnPOINodeActivated?.Invoke(this);
        }

        /// <summary>
        /// Invoked by the <see cref="POIManager"/> when another node is activated.
        /// </summary>
        public override void Deactivate()
        {
            if (!isActivated) return;

            isActivated = false;
        }
        #endregion
        
        protected override void SetAndCheckReferences()
        {
            base.SetAndCheckReferences();
            _rotator = FindObjectOfType<PassiveModeRotator>();
            Assert.IsNotNull(_rotator, $"{gameObject.name} cannot find PassiveModeRotator in scene.");
        }
    }
}