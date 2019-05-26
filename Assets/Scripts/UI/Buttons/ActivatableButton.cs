using UnityEngine;

namespace GLEAMoscopeVR.Interaction
{
    /// <summary>
    /// PURPOSE OF SCRIPT GOES HERE 
    /// </summary>
    public abstract class ActivatableButton : MonoBehaviour, IActivatable
    {
        public abstract float ActivationTime { get; }
        public abstract bool IsActivated { get; }

        public abstract void Activate();
        public abstract bool CanActivate();
        public abstract void Deactivate();
    }
}