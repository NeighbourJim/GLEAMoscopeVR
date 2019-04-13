using System;
using GLEAMoscopeVR.Utility.Management;
using UnityEngine;

namespace GLEAMoscopeVR.Settings
{
    /// <summary>
    /// <see cref="InteractionMode"/> 
    /// </summary>
    /// <summary>
    /// 20190413 MM - Added to assist with enabling and disabling functionality
    /// until a decision is made as to whether there will be separate modes
    /// or mixed functionality. For the sake of simplicity, the first iteration 02
    /// prototype will separate Exploration and Passive modes.
    /// They can be toggled (resetting functionality to the appropriate state),
    /// but they will exist as separate systems.
    /// </summary>
    public class InteractionModeController : GenericSingleton<InteractionModeController>
    {

        private const InteractionMode DEFAULT_MODE = InteractionMode.Exploration;

        [SerializeField]
        private InteractionMode currentMode;
        public InteractionMode CurrentMode => currentMode;

        /// <summary>
        /// Notifies dependent scripts that the interaction mode has changed.
        /// </summary>
        public event Action<InteractionMode> OnInteractionModeChanged;

        protected override void Awake()
        {
            currentMode = DEFAULT_MODE;
        }

        public void ChangeMode(bool isPassiveMode)
        {
            currentMode = isPassiveMode ? InteractionMode.Passive : InteractionMode.Exploration;
            OnInteractionModeChanged?.Invoke(currentMode);
        }
    }
}