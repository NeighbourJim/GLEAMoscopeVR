using GLEAMoscopeVR.Events;
using GLEAMoscopeVR.Interaction;
using GLEAMoscopeVR.Settings;
using GLEAMoscopeVR.Spectrum;
using UnityEngine;

namespace GLEAMoscopeVR.POIs
{
    /// <summary> Specifies behaviour for Point of Interest nodes in the sky. </summary>
    [SelectionBase]
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

        void Start()
        {
            _renderer.enabled = false;
            _collider.enabled = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EventManager.Instance.AddListener<SpectrumStateChangedEvent>(_ => SetVisibleState());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EventManager.Instance.RemoveListener<SpectrumStateChangedEvent>(_ => SetVisibleState());
        }
        
        #region IActivatable Implementation
        /// <summary> 
        /// Specifies whether this GameObject can currently be activated. 
        /// </summary>
        public override bool CanActivate()
        {
            return activatableMode == SettingsManager.Instance.CurrentExperienceMode && !isActivated;
        }

        /// <summary> 
        /// Invoked by the <see cref="CameraRayCaster"/> when the reticle loading process is complete.
        /// </summary>
        public override void Activate()
        {
            if (!CanActivate()) return;
            
            isActivated = true;
            EventManager.Instance.Raise(new POINodeActivatedEvent(this, $"Sky node activated: {Data.Name}"));
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

        protected override void SetVisibleState()
        {
            var validWavelength = SpectrumStateController.Instance.CurrentWavelength == Wavelength.Visible ||
                                  SpectrumStateController.Instance.CurrentWavelength == Wavelength.Radio;
            var validMode = SettingsManager.Instance.CurrentExperienceMode == ActivatableMode;

            _renderer.enabled = validMode && validWavelength;
            _collider.enabled = validMode && validWavelength;
        }
    }
}

//private bool introSequencePlaying = false;
//public void SetActivatableForIntroductionSequence()
//{
//    introSequencePlaying = true;
//    _renderer.enabled = true;
//    _collider.enabled = true;
//}