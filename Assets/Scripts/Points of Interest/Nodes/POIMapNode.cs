using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.Settings;
using UnityEngine;

namespace GLEAMoscopeVR.POIs
{
    /// <summary>
    /// War-table map point of interest nodes.
    /// </summary>
    public class POIMapNode : POINode
    {
        #region Constants
        private const ExperienceMode activatableMode = ExperienceMode.Passive;
        #endregion

        #region Public Accessors
        public override ExperienceMode ActivatableMode => activatableMode;
        public override bool IsActivated => isActivated;
        public override POIData Data => data;
        public override float ActivationTime => activationTime;
        #endregion
        
        void Start()
        {
            _renderer.enabled = false;
            _collider.enabled = false;
        }

        protected override void SetVisibleState()
        {
            var visible = SettingsManager.Instance.CurrentExperienceMode == ActivatableMode;
            _renderer.enabled = visible;
            _collider.enabled = visible;
        }

        #region IActivatable Implementation
        /// <summary> 
        /// Specifies whether this GameObject can currently be activated. 
        /// </summary>
        public override bool CanActivate()
        {
            return activatableMode == SettingsManager.Instance.CurrentExperienceMode && !isActivated && AppManager.Instance.RotatorCanSetTarget;
        }

        /// <summary>
        /// Invoked by the <see cref="CameraRayCaster"/> when the reticle loading process is complete.
        /// Triggers rotation of the Point of Interest into the user's view and notifies the <see cref="POIManager"/>
        /// that it has been activated.
        /// </summary>
        public override void Activate()
        {
            if (!CanActivate()) return;
            
            isActivated = true;
            EventManager.Instance.Raise(new POINodeActivatedEvent(this, $"Map node activated: {Data.Name}"));
        }
        #endregion

        protected override void HandleExperienceModeChanged(ExperienceModeChangedEvent e)
        {
            if(isActivated)
            {
                Deactivate();
            }
            SetVisibleState();
        }
    }
}