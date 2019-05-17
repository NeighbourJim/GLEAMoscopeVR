using System;
using GLEAMoscopeVR.Settings;
using UnityEngine;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// Specifies behaviour for Point of Interest nodes in the sky.
    /// Todo: OnNodeDeactivated
    /// </summary>
    public class POISkyNode : POINode
    {
        #region Constants
        private const ExperienceMode activatableMode = ExperienceMode.Exploration;
        #endregion

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

        #region IActivatable Implementation
        /// <summary> Specifies whether this GameObject can currently be activated. </summary>
        public override bool CanActivate()
        {
            return activatableMode == _modeController.CurrentMode && !isActivated;
        }

        /// <summary>
        /// Invoked by the <see cref="Script_CameraRayCaster"/> when the reticle loading process is complete.
        /// Notifies the <see cref="POIManager"/> that it has been activated.
        /// </summary>
        public override void Activate()
        {
            if (!CanActivate())
            {
                Debug.LogWarning($"[POISkyNode] {gameObject.name} is trying to activate when it shouldn't be able to.");
                Debug.Break();
                return;
            }

            isActivated = true;
            OnPOINodeActivated?.Invoke(this);
        }

        /// <summary>
        /// Invoked by the <see cref="POIManager"/> when another node is activated.
        /// </summary>
        public override void Deactivate()
        {
            if (!isActivated)
            {
                Debug.Log($"[POISkyNode] {gameObject.name} is attempting to deactivate but it is not activated.");
                return;
            }

            isActivated = false;
        }
        #endregion
    }
}