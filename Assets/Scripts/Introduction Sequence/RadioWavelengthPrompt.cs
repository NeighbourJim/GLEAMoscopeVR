using UnityEngine;
using UnityEngine.Assertions;

namespace GLEAMoscopeVR.Interaction
{
    public class RadioWavelengthPrompt : MonoBehaviour, IAnimatedPrompt
    {
        [HideInInspector] public string IdleTrigger => "SetIdle";
        [HideInInspector] public string AnimateTrigger => "Bounce";
        
        #region References
        Animator _animator = null;
        Renderer _renderer = null;
        public Renderer Renderer => _renderer;
        public Animator Animator => _animator;
        #endregion

        #region Unity Methods
        void Awake()
        {
            SetAndCheckReferences();
            TogglePrompt(false);
        }
        #endregion

        public void TogglePrompt(bool shouldPrompt)
        {
            _renderer.enabled = shouldPrompt;
            _animator.enabled = shouldPrompt;
            _animator.SetTrigger(shouldPrompt ? AnimateTrigger : IdleTrigger);
        }
        
        private void SetAndCheckReferences()
        {
            _animator = GetComponent<Animator>();
            Assert.IsNotNull(_animator, $"<b>[{GetType().Name}]</b> has no animator component.");

            _renderer = GetComponentInChildren<Renderer>();
            Assert.IsNotNull(_renderer, $"<b>[{GetType().Name}]</b> has no renderer component.");
        }
    }
}