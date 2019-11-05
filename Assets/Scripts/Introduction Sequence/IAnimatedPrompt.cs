using UnityEngine;

namespace GLEAMoscopeVR.Interaction
{
    public interface IAnimatedPrompt
    {
        string IdleTrigger { get; }
        string AnimateTrigger { get; }

        Renderer Renderer { get; }
        Animator Animator { get; }

        void TogglePrompt(bool shouldPrompt);
    }
}